@import GoogleSignIn;

void LoginGoogleInIos(const char* iosClientId, const char* gameObjName, const char* funcSuccessName, const char* funcFailName)
{
    NSString* iosClientIdStr = [NSString stringWithUTF8String:iosClientId];
    NSString* gameObjNameStr = [NSString stringWithUTF8String:gameObjName];
    NSString* funcSuccessNameStr = [NSString stringWithUTF8String:funcSuccessName];
    NSString* funcFailNameStr = [NSString stringWithUTF8String:funcFailName];
    
    GIDConfiguration *config = [[GIDConfiguration alloc] initWithClientID:iosClientIdStr];
    [GIDSignIn.sharedInstance setConfiguration:config];
    
    UIViewController* unityVC = UnityGetGLViewController();
    [GIDSignIn.sharedInstance signInWithPresentingViewController:unityVC
                                                      completion:^(GIDSignInResult * _Nullable result, NSError * _Nullable error) {
        if (error == nil) {
            UnitySendMessage([gameObjNameStr UTF8String], [funcSuccessNameStr UTF8String], [result.user.idToken.tokenString UTF8String]);
        }
        else{
            NSString *errorMessage;
            if ([error.domain isEqualToString:@"com.google.GIDSignIn"]) {
                switch (error.code) {
                    case -1:
                        errorMessage = @"an unknown error has occurred.";
                        break;
                    case -2:
                        errorMessage = @"a problem reading or writing to the application keychain.";
                        break;
                    case -4:
                        errorMessage = @"there are no valid auth tokens in the keychain.";
                        break;
                    case -5:
                        errorMessage = @"the user canceled the sign in request.";
                        break;
                    case -6:
                        errorMessage = @"an Enterprise Mobility Management related error has occurred.";
                        break;
                    case -8:
                        errorMessage = @"the requested scopes have already been granted to the currentUser.";
                        break;
                    case -9:
                        errorMessage = @"there is an operation on a previous user.";
                        break;
                    default:
                        errorMessage = [NSString stringWithFormat:@"Google Sign-In error (Code: %ld)", (long)error.code];
                        break;
                }
            } else {
                errorMessage = [NSString stringWithFormat:@"Sign-in failed: %@ (Code: %ld)", error.domain, (long)error.code];
            }
            UnitySendMessage([gameObjNameStr UTF8String], [funcFailNameStr UTF8String], [errorMessage UTF8String]);
        }
    }];
}
