#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

void _forceToSpeaker() {
    UInt32 routeSize = sizeof(CFStringRef);
    CFStringRef route = NULL;
    OSStatus error = AudioSessionGetProperty(kAudioSessionProperty_AudioRoute, &routeSize, &route);

    if (!error && (route != NULL)&&
        ([(__bridge NSString*)route rangeOfString:@"Head"].location != NSNotFound)) {
        //CFRelease(route);
        NSLog(@"Headset connected!");
        return;
    }

    UInt32 audioRouteOverride = kAudioSessionOverrideAudioRoute_Speaker;
    error = AudioSessionSetProperty(kAudioSessionProperty_OverrideAudioRoute,
                                     sizeof(audioRouteOverride),
                                     &audioRouteOverride);

    if (error) {
        NSLog(@"Audio already playing through speaker!");
    } else {
        NSLog(@"Forcing audio to speaker");
    }
}
