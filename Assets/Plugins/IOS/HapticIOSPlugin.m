//import the header file.
#import "HapticIOSPlugin.h"
#import "AVFoundation/AVFoundation.h"

@interface HapticIOSPlugin ()
{
    UIImpactFeedbackGenerator *light;
    UIImpactFeedbackGenerator *medium;
    UIImpactFeedbackGenerator *heavy;
    
}
@end

@implementation HapticIOSPlugin

- (id)init
{
    light = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
    medium = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
    heavy = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
    return self;
}
- (void)Light
{
    [light impactOccurred];
}

- (void)Medium
{
    [medium impactOccurred];
}


- (void)Heavy
{
    [heavy impactOccurred];
    NSLog(@"DEBUG: Heavy");

}

@end

extern "C"{
    HapticIOSPlugin *haptic = [[HapticIOSPlugin alloc] init];
    void _Light(){
        [haptic Light];
    }
    void _Medium(){
        [haptic Medium];
    }
    void _Heavy(){
        [haptic Heavy];
    }
}

