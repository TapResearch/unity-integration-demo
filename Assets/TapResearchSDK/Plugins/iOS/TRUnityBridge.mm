//
//  TRUnityBridge.mm
//  TRUnityBridge - iOS
//
//  Created by Kevin Chang on 7/13/16.
//
//

#import <TapResearchSDK/TapResearchSDK.h>
#import <TapResearchSDK/TRPlacementCustomParameter.h>
#import <TapResearchSDK/TRPlacementCustomParameterList.h>
#import <TapResearchSDK/TRPlacementCustomParameter+Builder.h>

BOOL receiveRewardCollection = NO;

//MARK: - Delegate class for TapResearchSDK

@interface TRUnityDelegate : NSObject<TapResearchRewardDelegate, TapResearchSurveyDelegate, TapResearchPlacementDelegate, TapResearchEventDelegate> {
}

@property (readwrite, nonatomic) NSMutableDictionary *placements;

- (void)tapResearchDidReceiveRewards:(NSArray *)rewards;
- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement;
- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement;
- (void)tapResearchEventOpenedWithPlacement:(TRPlacement *)placement;
- (void)tapResearchEventDismissedWithPlacement:(TRPlacement *)placement;
- (void)placementReady:(TRPlacement *)placement;
- (void)placementUnavailable:(NSString *)placementId;

@end

//MARK: - Delegate class implementation for TapResearchSDK

@implementation TRUnityDelegate

//MARK: - Rewards delegate

- (void)tapResearchDidReceiveRewards:(NSArray<TRReward *>*)rewards {
	
	// Send an Array of rewards to Unity
	NSMutableArray *values = [[NSMutableArray alloc] init];
	for (TRReward *reward in rewards) {
		NSDictionary *rewardDict = [TRSerializationHelper dictionaryWithPropertiesOfObject: reward];
		[values addObject:rewardDict];
	}
	NSString *jsonString = [TRSerializationHelper jsonStringFromArray:values];
	UnitySendMessage("TapResearch", "OnTapResearchDidReceiveRewardCollection", [jsonString UTF8String]);
}

//MARK: - Survey wall opened/dismissed delegates

- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement {
	[self sendPlacement:placement message:@"TapResearchOnSurveyWallOpened"];
}

- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement {
	[self sendPlacement:placement message:@"TapResearchOnSurveyWallDismissed"];
}

//MARK: - Event opened/dismissed delegates

- (void)tapResearchEventOpenedWithPlacement:(TRPlacement *)placement {
	[self sendPlacement:placement message:@"TapResearchOnEventOpened"];
}

- (void)tapResearchEventDismissedWithPlacement:(TRPlacement *)placement {
	[self sendPlacement:placement message:@"TapResearchOnEventDismissed"];
}

//MARK: - TRPlacement availability delegates

- (void)placementReady:(nonnull TRPlacement *)placement {
	
	if (!self.placements) {
		self.placements = [[NSMutableDictionary alloc] init];
	}
	[self.placements setObject:placement forKey:placement.placementIdentifier];
	[self sendPlacement:placement message:@"OnTapResearchPlacementReady"];
}

- (void)placementUnavailable:(nonnull NSString *)placementId {
	[self.placements removeObjectForKey:placementId];
	UnitySendMessage("TapResearch", [@"OnTapResearchPlacementUnavailable" UTF8String], [placementId UTF8String]);
}

//MARK: - Message sender *to* Untiy C#

- (void)sendPlacement:(TRPlacement *)placement message:(NSString *)message {
    
	NSMutableArray *events = [[NSMutableArray alloc] init];
	if (placement.events.count > 0) {
		for (TREvent *event in placement.events) {
			NSDictionary *eventDict = [TRSerializationHelper dictionaryWithPropertiesOfObject:event];
			[events addObject:eventDict];
		}
	}
	
	NSMutableDictionary *placementDict = [NSMutableDictionary dictionaryWithDictionary:[TRSerializationHelper dictionaryWithPropertiesOfObject:placement]];
	placementDict[@"events"] = events;
	
    NSString *jsonString = [TRSerializationHelper jsonStringFromDictionary:placementDict];
    UnitySendMessage("TapResearch", [message UTF8String], [jsonString UTF8String]);
}

@end

//MARK: - C interfaces for Unity/C#

#include <iostream>
using namespace std;

#define DEV_PLATFORM @"unity"

NSString *iOSToken;
TRUnityDelegate *iOSDelegate = nil;
BOOL configured = NO;

//MARK: - Start of externals
extern "C" {

void TRIOSConfigure(const char *apiToken, const char *version) {

    if (configured) {
        return;
    }
    iOSToken = [NSString stringWithUTF8String:apiToken];
    NSString *versionString = [NSString stringWithUTF8String:version];
    iOSDelegate = [[TRUnityDelegate alloc] init];
    [TapResearch initWithApiToken:iOSToken developmentPlatform:DEV_PLATFORM developmentPlatformVersion:versionString rewardDelegate:iOSDelegate placementDelegate:iOSDelegate];
    configured = YES;
}

void SetUniqueUserIdentifier(const char *userIdentifier) {

    NSString *identifier = [NSString stringWithUTF8String:userIdentifier];
    [TapResearch setUniqueUserIdentifier:identifier];
}

//MARK: - Set reward return type (array or single-in-loop)

TRPlacementCustomParameterList *InitPlacementParameters(const char *customParameters) {
    
     NSString *param = [NSString stringWithUTF8String:customParameters];
     NSError *jsonError;
     NSData *objectData = [param dataUsingEncoding:NSUTF8StringEncoding];
     NSDictionary *json = [NSJSONSerialization JSONObjectWithData:objectData
                                           options:NSJSONReadingMutableContainers
                                             error:&jsonError];
     NSArray *arr = [json objectForKey:@"ParameterList"];
     TRPlacementCustomParameterList *parameterList = [[TRPlacementCustomParameterList alloc] init];
     for(NSDictionary *dict in arr) {
         TRPlacementCustomParameter *param = [TRPlacementCustomParameter new];
         NSString *key = [dict objectForKey: @"key"];
         NSString *value = [dict objectForKey: @"value"];
         [[[[param builder] key: key] value: value] build];
         [parameterList addParameter:param];
     }
     
     return parameterList;
}

//MARK: - Survey wall presenters

BOOL IsEventAvailable(const char* placementIdentifier) {
    
	NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
    TRPlacement *placement = iOSDelegate.placements[placementIdentifierString];
    if (placement) {
        return [placement isEventAvailable];
    }
	else {
		return false;
	}
}

//MARK: - Survey wall presenters

void ShowSurveyWall(const char* placementIdentifier) {
    
    NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
    TRPlacement *placement = iOSDelegate.placements[placementIdentifierString];
    if (placement) {
        [placement showSurveyWallWithDelegate:iOSDelegate];
    }
}

void ShowSurveyWallWithParameters(const char* placementIdentifier, const char *customParameters) {
    
    TRPlacementCustomParameterList *parameterList;
    if(customParameters) {
        parameterList = InitPlacementParameters(customParameters);
    }
    
    NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
    TRPlacement *placement = iOSDelegate.placements[placementIdentifierString];
    if (placement) {
        [placement showSurveyWallWithDelegate:iOSDelegate customParameters:parameterList];
    }
}

//MARK: - Event presenters

void DisplayEvent(const char* placementIdentifier) {
	
	NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
	TRPlacement *placement = iOSDelegate.placements[placementIdentifierString];
	if (placement && placement.events.count > 0) {
		TREvent *event = placement.events[0];
		if (event) {
			[placement displayEvent:event.identifier withDisplayDelegate:iOSDelegate surveyDelegate:iOSDelegate customParameters:nil];
		}
	}
}

void DisplayEventWithParameters(const char* placementIdentifier, const char *customParameters) {
	
	TRPlacementCustomParameterList *parameterList;
	if(customParameters) {
		parameterList = InitPlacementParameters(customParameters);
	}
	
	NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
	TRPlacement *placement = iOSDelegate.placements[placementIdentifierString];
	if (placement && placement.events.count > 0) {
		TREvent *event = placement.events[0];
		if (event) {
			[placement displayEvent:event.identifier withDisplayDelegate:iOSDelegate surveyDelegate:iOSDelegate customParameters:nil];
		}
	}
}

//MARK: End of externals
} /* End of externals */
//MARK: -
