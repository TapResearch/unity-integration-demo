//
//  TRUnityBridge.mm
//  TRUnityBridge - iOS
//
//  Created by Kevin Chang on 7/13/16.
//
//

#import <TapResearchSDK/TapResearchSDK.h>

@interface TRUnityDelegate : NSObject<TapResearchDelegate, TapResearchSurveyDelegate> {
    
}

- (void)tapResearchDidReceiveRewardWithQuantity:(NSInteger)quantity transactionIdentifier:(NSString *)transactionIdentifier currencyName:(NSString *)currencyName payoutEvent:(NSInteger)payoutEvent offerIdentifier:(NSString *) offerIdentifier;
- (void)tapResearchSurveyModalOpened;
- (void)tapResearchSurveyModalDismissed;
- (void)tapResearchOnSurveyAvailable;
- (void)tapResearchOnSurveyNotAvailable;



@end

@implementation TRUnityDelegate

- (void)tapResearchDidReceiveRewardWithQuantity:(NSInteger)quantity transactionIdentifier:(NSString *)transactionIdentifier currencyName:(NSString *)currencyName payoutEvent:(NSInteger)payoutEvent offerIdentifier:(NSString *)offerIdentifier;

{
    const char *message = [[NSString stringWithFormat:@"%ld|%@|%@|%ld|%@", (long)quantity, transactionIdentifier, currencyName, payoutEvent, offerIdentifier] UTF8String];
    UnitySendMessage("TapResearch", "OnTapResearchDidReceiveReward", message);
}

- (void)tapResearchSurveyModalOpened
{
    UnitySendMessage("TapResearch", "OnTapResearchSurveyModalOpened", [@"" UTF8String]);
}

- (void)tapResearchSurveyModalDismissed
{
    UnitySendMessage("TapResearch", "OnTapResearchSurveyModalDismissed", [@"" UTF8String]);
}

- (void)tapResearchOnSurveyAvailable
{
    UnitySendMessage("TapResearch", "OnTapResearchSurveyAvailable", [@"" UTF8String]);
}

- (void)tapResearchOnSurveyNotAvailable
{
    UnitySendMessage("TapResearch", "OnTapResearchSurveyNotAvailable", [@"" UTF8String]);
}



@end

#include <iostream>
using namespace std;

NSString *iOSToken;
TRUnityDelegate *iOSDelegate = nil;
BOOL configured = NO;

extern "C" {
    void TRIOSConfigure(const char *apiToken) {
        if (configured) return;
        
        iOSToken = [NSString stringWithUTF8String:apiToken];
        iOSDelegate = [[TRUnityDelegate alloc] init];
        
        [TapResearch initWithApiToken:iOSToken delegate:iOSDelegate];
        configured = YES;
    }
    
    bool IsSurveyAvailable() {
        if (!configured) return NO;
        return [TapResearch isSurveyAvailable];
    }
    
    void ShowSurvey() {
        [TapResearch showSurveyWithDelegate:iOSDelegate];
    }
    
    void ShowSurveyWithIdentifier(const char *identifier) {
        NSString *iOSidentifier = identifier ? [NSString stringWithUTF8String:identifier] : nil;
        [TapResearch showSurveyWithIdentifier:iOSidentifier delegate:iOSDelegate];
    }
    
    void SetUniqueUserIdentifier(const char *userIdentifier) {
        NSString *identifier = [NSString stringWithUTF8String:userIdentifier];
        [TapResearch setUniqueUserIdentifier:identifier];
    }
}
