//
//  TRUnityBridge.mm
//  TRUnityBridge - iOS
//
//  Created by Kevin Chang on 7/13/16.
//
//

#import <TapResearchSDK/TapResearchSDK.h>

@interface TRUnityDelegate : NSObject<TapResearchRewardDelegate, TapResearchSurveyDelegate> {

}

- (void)tapResearchDidReceiveReward:(TRReward *)reward;
- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement;
- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement;

@end

@implementation TRUnityDelegate

- (void)tapResearchDidReceiveReward:(TRReward *)reward;
{
    NSDictionary *rewardDict = [TRSerilizationHelper dictionaryWithPropertiesOfObject: reward];
    NSString *jsonString = [TRSerilizationHelper jsonStringFromDictionary:rewardDict];
    UnitySendMessage("TapResearch", "OnTapResearchDidReceiveReward", [jsonString UTF8String]);
}

- (void)tapResearchSurveyWallOpenedWithPlacement:(TRPlacement *)placement;
{
    [self sendPlacement:placement message:@"TapResearchOnSurveyWallOpened"];
}

- (void)tapResearchSurveyWallDismissedWithPlacement:(TRPlacement *)placement;
{
    [self sendPlacement:placement message:@"TapResearchOnSurveyWallDismissed"];
}

- (void)sendPlacement:(TRPlacement *)placement message:(NSString *)message
{
    NSDictionary *placementDict = [TRSerilizationHelper dictionaryWithPropertiesOfObject:placement];
    NSString *jsonString = [TRSerilizationHelper jsonStringFromDictionary:placementDict];
    UnitySendMessage("TapResearch", [message UTF8String], [jsonString UTF8String]);
}

@end

#include <iostream>
using namespace std;

#define DEV_PLATFORM @"unity"

NSString *iOSToken;
TRUnityDelegate *iOSDelegate = nil;
BOOL configured = NO;
NSMutableDictionary *placementsCache = [[NSMutableDictionary alloc]init];

UIColor *colorFromHexString(const char *hexColor) {
    unsigned rgbValue = 0;
    NSString *hexString = [NSString stringWithUTF8String:hexColor];
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1];
    [scanner scanHexInt:&rgbValue];

    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}


extern "C" {
    void TRIOSConfigure(const char *apiToken, const char *version) {
        if (configured) return;

        iOSToken = [NSString stringWithUTF8String:apiToken];
        NSString *versionString = [NSString stringWithUTF8String:version];
        iOSDelegate = [[TRUnityDelegate alloc] init];
        [TapResearch initWithApiToken:iOSToken developmentPlatform:DEV_PLATFORM developmentPlatformVersion:versionString delegate:iOSDelegate];
        configured = YES;
    }

    void SetUniqueUserIdentifier(const char *userIdentifier) {
        NSString *identifier = [NSString stringWithUTF8String:userIdentifier];
        [TapResearch setUniqueUserIdentifier:identifier];
    }

    void InitPlacement(const char *placementIdentifier) {
        NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
        [TapResearch initPlacementWithIdentifier:placementIdentifierString placementBlock:^(TRPlacement *placement) {
            [placementsCache setObject:placement forKey:placement.placementIdentifier];
            NSDictionary *placementDict = [TRSerilizationHelper dictionaryWithPropertiesOfObject: placement];
            NSString *jsonString = [TRSerilizationHelper jsonStringFromDictionary:placementDict];
            UnitySendMessage("TapResearch", "OnTapResearchPlacementReady", [jsonString UTF8String]);
        }];
    }

    void ShowSurveyWall(const char* placementIdentifier) {
        NSString *placementIdentifierString = [NSString stringWithUTF8String:placementIdentifier];
        TRPlacement *placement = [placementsCache valueForKey:placementIdentifierString];
        if (placement) {
            [placement showSurveyWallWithDelegate:iOSDelegate];
        }
    }

    void SetNavigationBarColor(const char *hexColor) {
        if (!hexColor) return;
        UIColor *color = colorFromHexString(hexColor);
        [TapResearch setNavigationBarColor:color];
    }

    void SetNavigationBarText(const char *text) {
        if (!text) return;
        NSString *titleText = [NSString stringWithUTF8String:text];
        [TapResearch setNavigationBarText:titleText];
    }

    void SetNavigationBarTextColor(const char *hexColor) {
        if (!hexColor) return;
        UIColor *color = colorFromHexString(hexColor);
        [TapResearch setNavigationBarTextColor:color];
    }

}
