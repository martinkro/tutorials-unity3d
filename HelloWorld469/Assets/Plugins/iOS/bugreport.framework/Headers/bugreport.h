//
//  bugreport.h
//  bugreport
//
//  Created by gavingu on 16/7/4.
//  Copyright © 2016年 tencent. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for bugreport.
FOUNDATION_EXPORT double bugreportVersionNumber;

//! Project version string for bugreport.
FOUNDATION_EXPORT const unsigned char bugreportVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <bugreport/PublicHeader.h>

@interface BugReport:NSObject

+(instancetype) sharedInstance;

-(void)start;

-(void)stop;

-(void)setAppId:(NSString*)appId;

-(void)setUserId:(NSString*)userId;

-(void)setEnableDebugLog:(bool)enableDebugLog;

@end

#ifdef __cplusplus
extern "C"{
#endif
    
    void __attribute__((visibility("default"))) BugReportInit();
    void __attribute__((visibility("default"))) BugReportSetUserId(const char* userId);
    void __attribute__((visibility("default"))) BugReportSetAppId(const char* appId);
    void __attribute__((visibility("default"))) BugReportSetEnableDebugLog(bool enableDebugLog);
    void __attribute__((visibility("default"))) BugReportSendCSharpException(const char* reason, const char* stack);

#ifdef __cplusplus
}
#endif


