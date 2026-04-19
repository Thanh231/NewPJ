
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>

extern "C"
{
    void RequestAppTrackingTransparencyPermission()
    {
        if (@available(iOS 14, *))
        {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status){
                switch (status){
                    case ATTrackingManagerAuthorizationStatusAuthorized:
                        UnitySendMessage("app-tracking-transparency", "DoneRequestPermission", "Authorized");
                        break;
                    case ATTrackingManagerAuthorizationStatusDenied:
                        UnitySendMessage("app-tracking-transparency", "DoneRequestPermission", "Denied");
                        break;
                    case ATTrackingManagerAuthorizationStatusRestricted:
                        UnitySendMessage("app-tracking-transparency", "DoneRequestPermission", "Restricted");
                        break;
                    case ATTrackingManagerAuthorizationStatusNotDetermined:
                        UnitySendMessage("app-tracking-transparency", "DoneRequestPermission", "NotDetermined");
                        break;
                }
            }];
        }
        else
        {
            UnitySendMessage("app-tracking-transparency", "DoneRequestPermission", "Authorized");
        }
    }
}