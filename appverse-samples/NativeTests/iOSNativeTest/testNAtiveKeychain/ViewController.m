//
//  ViewController.m
//  testNAtiveKeychain
//
//  Created by David Barranco on 9/3/13.
//  Copyright (c) 2013 David Barranco. All rights reserved.
//

#import "ViewController.h"

@interface ViewController ()

@end

@implementation ViewController

//static NSString *serviceName = @"Appverse_Keychain";
//static NSString *labelName = @"Appverse Label";
//static NSString *genericName = @"Appverse Generic";

static NSString *accessGroup = @"YOUR_ACCESS_GROUP_NAME"; //Example: 123456.com.gft.appverse.Application1

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
}

-(NSMutableDictionary *) newSearchDictionary:(NSString *)identifier{
    
    NSMutableDictionary *searchDictionary = [[NSMutableDictionary alloc] init];
    [searchDictionary setObject:(__bridge id)kSecClassGenericPassword forKey:(__bridge id)kSecClass];
    
    [searchDictionary setObject:identifier forKey:(__bridge id)kSecAttrAccount];
	//LABEL, SERVICE and GENERIC fields are not used
    //[searchDictionary setObject:labelName forKey:(__bridge id)kSecAttrLabel];
    //[searchDictionary setObject:serviceName forKey:(__bridge id)kSecAttrService];
	//[searchDictionary setObject:genericName forKey:(__bridge id)kSecAttrGeneric];
    [searchDictionary setObject:accessGroup forKey:(__bridge id)kSecAttrAccessGroup];
    
    return searchDictionary;
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@synthesize keyName = _keyName;

- (IBAction)readKeychain:(id)sender {
    self.keyName = self.textKeyName.text;
    
    NSString *nameString = self.keyName;
    if([nameString length] == 0){
        self.resultLabel.text = @"Type some text";
    }else{
        NSData *valueData = [self searchKeychainCopyMatching:nameString];
        if(valueData){
            NSString *value = [[NSString alloc] initWithData:valueData encoding:NSUTF8StringEncoding];

            NSString *showValue = [[NSString alloc] initWithFormat:@"%@ %@",self.keyName,value];
            self.resultLabel.text = showValue;
        }else{
            self.resultLabel.text = [@"Key not found: " stringByAppendingString:self.keyName];
        }
    }
}

- (IBAction)createKeychain:(id)sender {
    self.keyName = self.textKeyName.text;
    
    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    NSString *labelValue = [@"Really " stringByAppendingString:bundleIdentifier];
    
    NSString *identifier = self.keyName;
    [self createOrUpdateKeychainValue:labelValue forIdentifier:identifier];
}

- (IBAction)deleteKeychain:(id)sender {
    self.keyName = self.textKeyName.text;
    
    NSString *identifier = self.keyName;
    NSMutableDictionary *searchDictionary = [self newSearchDictionary:identifier];
    OSStatus status = SecItemDelete((__bridge CFDictionaryRef)searchDictionary);
    

    if(status == errSecSuccess){
        self.resultLabel.text = @"Key Removed Successfully";
    }else{
        self.resultLabel.text = @"Could not remove key: ";
    }
}



- (NSData *)searchKeychainCopyMatching:(NSString *) identifier{
    NSMutableDictionary *searchDictionary = [self newSearchDictionary:identifier];
    //Add search return type
    [searchDictionary setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id)kSecReturnData];
    
    CFTypeRef result = NULL;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)searchDictionary, &result);
    NSData *data = (__bridge NSData *)result;
    return data;
    
}

-(void) createOrUpdateKeychainValue:(NSString *)value forIdentifier:(NSString *)identifier{
    NSMutableDictionary *dictionary = [self newSearchDictionary:identifier];
    NSData *valueData = [value dataUsingEncoding:NSUTF8StringEncoding];
    [dictionary setObject:valueData forKey:(__bridge id)kSecValueData];
    [dictionary setObject:(__bridge id)kSecAttrAccessibleAlways forKey:(__bridge id)kSecAttrAccessible];
    
    OSStatus status = SecItemAdd((__bridge CFDictionaryRef)dictionary, NULL);

    switch (status) {
            
        case errSecAllocate:
            self.resultLabel.text = @"1";
            break;
        case errSecAuthFailed:
            self.resultLabel.text = @"2";
            break;
        case errSecBadReq:
            self.resultLabel.text = @"3";
            break;
        case errSecDecode:
            self.resultLabel.text = @"4";
            break;
        case errSecDuplicateItem:
            self.resultLabel.text = @"5";
            break;
        case errSecInteractionNotAllowed:
            self.resultLabel.text = @"6";
            break;
        case errSecInternalComponent:
            self.resultLabel.text = @"7";
            break;
        case errSecIO:
            self.resultLabel.text = @"8";
            break;
        case errSecItemNotFound:
            self.resultLabel.text = @"9";
            break;
        case errSecNotAvailable:
            self.resultLabel.text = @"10";
            break;
        case errSecOpWr:
            self.resultLabel.text = @"11";
            break;
        case errSecParam:
            self.resultLabel.text = @"12";
            break;
        case errSecSuccess:
            self.resultLabel.text = @"13";
            break;
        case errSecUnimplemented:
            self.resultLabel.text = @"14";
            break;
        case errSecUserCanceled:
            self.resultLabel.text = @"15";
            break;
        default:
            break;
    }

    if(status == errSecDuplicateItem){
        NSMutableDictionary *deleteEntry = [self newSearchDictionary:identifier];
        status = SecItemDelete((__bridge CFDictionaryRef) deleteEntry);
        if(status == errSecSuccess)
        {
            status = SecItemAdd((__bridge CFDictionaryRef)dictionary, NULL);
        }
    }
    if(status == errSecSuccess){
        self.resultLabel.text = @"Key Created Successfully";
    }else{
        self.resultLabel.text = @"Could not Create key";
    }
}



- (BOOL)textFieldShouldReturn:(UITextField *)theTextField{
    if(theTextField == self.textKeyName){
        [theTextField resignFirstResponder];
    }
    return YES;
}
@end
