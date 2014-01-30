//
//  ViewController.h
//  testNAtiveKeychain
//
//  Created by David Barranco on 9/3/13.
//  Copyright (c) 2013 David Barranco. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <Security/Security.h>

@interface ViewController : UIViewController <UITextFieldDelegate>

@property (weak, nonatomic) IBOutlet UITextField *textKeyName;
@property (copy, nonatomic) NSString *keyName;
@property (weak, nonatomic) IBOutlet UILabel *resultLabel;

- (IBAction)readKeychain:(id)sender;
- (IBAction)createKeychain:(id)sender;
- (IBAction)deleteKeychain:(id)sender;

@end
