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

@interface TRUnityDelegate : NSObject<TapResearchRewardDelegate, TapResearchSurveyDelegate, TapResearchPlacementDelegate> {
}

@property (readwrite, nonatomic) NSMutableDictionary *placements;

- (void)tapResearchDidReceiveReward:(TRReward *)reward;
- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement;
- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement;

@end


//MARK: - Delegate class implementation for TapResearchSDK


@implementation TRUnityDelegate

- (void)tapResearchDidReceiveReward:(TRReward *)reward {

    NSDictionary *rewardDict = [TRSerializationHelper dictionaryWithPropertiesOfObject: reward];
    NSString *jsonString = [TRSerializationHelper jsonStringFromDictionary:rewardDict];
    UnitySendMessage("TapResearch", "OnTapResearchDidReceiveReward", [jsonString UTF8String]);
}

- (void)tapResearchDidReceiveRewards:(NSArray<TRReward *>*)rewards {
    
    if (receiveRewardCollection) {
        // Send an Array of rewards to Unity
        NSMutableArray *values = [[NSMutableArray alloc] init];
        for (TRReward *reward in rewards) {
            NSDictionary *rewardDict = [TRSerializationHelper dictionaryWithPropertiesOfObject: reward];
            [values addObject:rewardDict];
        }
        NSString *jsonString = [TRSerializationHelper jsonStringFromArray:values];
        UnitySendMessage("TapResearch", "OnTapResearchDidReceiveRewardCollection", [jsonString UTF8String]);
    }
    else {
        // Send the rewards one-by-one to Unity (existing functionality)
        for (TRReward *reward in rewards) {
            [self tapResearchDidReceiveReward:reward];
        }
    }
}

- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement {
    [self sendPlacement:placement message:@"TapResearchOnSurveyWallOpened"];
}

- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement {
    [self sendPlacement:placement message:@"TapResearchOnSurveyWallDismissed"];
}

- (void)sendPlacement:(TRPlacement *)placement message:(NSString *)message {
    
    NSDictionary *placementDict = [TRSerializationHelper dictionaryWithPropertiesOfObject:placement];
    NSString *jsonString = [TRSerializationHelper jsonStringFromDictionary:placementDict];
    UnitySendMessage("TapResearch", [message UTF8String], [jsonString UTF8String]);
}

- (void)placementReady:(nonnull TRPlacement *)placement {

    if (!self.placements) {
        self.placements = [[NSMutableDictionary alloc] init];
    }
    [self.placements setObject:placement forKey:placement.placementIdentifier];
    [self sendPlacement:placement message:@"OnTapResearchPlacementEventReady"];
}

- (void)placementUnavailable:(nonnull NSString *)placementId {
    [self.placements removeObjectForKey:placementId];
    UnitySendMessage("TapResearch", [@"OnTapResearchPlacementEventUnavailable" UTF8String], [placementId UTF8String]);
}

@end

//MARK: - C interfaces for Unity/C#

#include <iostream>
using namespace std;

#define DEV_PLATFORM @"unity"

NSString *iOSToken;
TRUnityDelegate *iOSDelegate = nil;
BOOL configured = NO;

UIColor *colorFromHexString(const char *hexColor) {
  
    unsigned rgbValue = 0;
    NSString *hexString = [NSString stringWithUTF8String:hexColor];
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1];
    [scanner scanHexInt:&rgbValue];

    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}

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

void SetReceiveRewardCollection(int32_t receiveCollection) {
    receiveRewardCollection = (BOOL)receiveCollection;
}

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

void SetNavigationBarColor(const char *hexColor) {
    
    if (!hexColor) {
        return;
    }
    UIColor *color = colorFromHexString(hexColor);
    [TapResearch setNavigationBarColor:color];
}

void SetNavigationBarText(const char *text) {
    
    if (!text) {
        return;
    }
    NSString *titleText = [NSString stringWithUTF8String:text];
    [TapResearch setNavigationBarText:titleText];
}

void SetNavigationBarTextColor(const char *hexColor) {
    
    if (!hexColor) {
        return;
    }
    UIColor *color = colorFromHexString(hexColor);
    [TapResearch setNavigationBarTextColor:color];
}

//MARK: End of externals
} /* End of externals */
//MARK: -

