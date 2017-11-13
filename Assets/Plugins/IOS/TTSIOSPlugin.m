//import the header file.
#import "TTSIOSPlugin.h"
#import "AVFoundation/AVFoundation.h"

@interface TTSIOSPlugin () <AVSpeechSynthesizerDelegate>
{
    AVSpeechSynthesizer *speechSynthesizer;
    NSString * speakText;
    NSString * LanguageCode;
    float pitch;
    float rate;
    bool stopping;
}
@end

@implementation TTSIOSPlugin

- (id)init
{
    self = [super init];
    speechSynthesizer = [[AVSpeechSynthesizer alloc] init];
    speechSynthesizer.delegate = self;
    return self;
}
- (void)SettingSpeak: (const char *) _language pitchSpeak: (float)_pitch rateSpeak:(float)_rate
{
    LanguageCode = [NSString stringWithUTF8String:_language];
    pitch = _pitch;
    rate = _rate;
    NSLog(@"DEBUG: Setting Success");
    //UnitySendMessage("TTS", "onMessage", "Setting Success");
}
- (void)StartSpeak: (const char *) _text
{
    speakText = [NSString stringWithUTF8String:_text];
    AVSpeechUtterance *utterance = [[AVSpeechUtterance alloc] initWithString:speakText];
    utterance.voice = [AVSpeechSynthesisVoice voiceWithLanguage:LanguageCode];
    utterance.pitchMultiplier = pitch;
    utterance.rate = rate;
    utterance.preUtteranceDelay = 0.f;
    utterance.postUtteranceDelay = 0.2f;
    [speechSynthesizer speakUtterance:utterance];
}
- (void)StopSpeak
{
    NSLog(@"StopSpeak: %s", [speechSynthesizer isSpeaking] ? "true" : "false");
    
    if([speechSynthesizer isSpeaking]) {
        stopping = true;
        [speechSynthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
        //        AVSpeechUtterance *utterance = [AVSpeechUtterance speechUtteranceWithString:@""];
        //        [speechSynthesizer speakUtterance:utterance];
        //        [speechSynthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
        NSLog(@"DEBUG: stop called");
        
    }
}

- (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer
willSpeakRangeOfSpeechString:(NSRange)characterRange
                utterance:(AVSpeechUtterance *)utterance
{
    NSString *subString = [speakText substringWithRange:characterRange];
    NSLog(@"DEBUG: onSpeechRange  %@", subString);
    
    //UnitySendMessage("TTS", "onSpeechRange", [subString UTF8String]);
}

- (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer
  didStartSpeechUtterance:(AVSpeechUtterance *)utterance
{
    NSLog(@"DEBUG: onStart");
    //UnitySendMessage("TTS", "onStart", "onStart");
}

- (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer
 didFinishSpeechUtterance:(AVSpeechUtterance *)utterance
{
    NSLog(@"DEBUG: done");
    //UnitySendMessage("TTS", "onDone", "onDone");
}

- (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer
  didCancelSpeechUtterance:(AVSpeechUtterance *)utterance
{
    NSLog(@"DEBUG: canceled");
    stopping = false;
    //UnitySendMessage("TTS", "onStart", "onStart");
}

@end

extern "C"{
    TTSIOSPlugin *su = [[TTSIOSPlugin alloc] init];
    void _StartSpeak(const char * _text){
        [su StartSpeak:_text];
    }
    void _StopSpeak(){
        [su StopSpeak];
    }
    void _SettingSpeak(const char * _language, float _pitch, float _rate){
        [su SettingSpeak:_language pitchSpeak:_pitch rateSpeak:_rate];
    }
}

