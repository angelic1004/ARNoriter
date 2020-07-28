#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

void iOS_ShareWithApp(const char* text, const char* path)
{
    NSString* message = [NSString stringWithUTF8String: text];
    NSString* filePath = [NSString stringWithUTF8String: path];
	NSURL* fileURL = [NSURL fileURLWithPath: filePath];
    NSArray* postItems = @[message, fileURL];

    UIActivityViewController* activityVc = [[UIActivityViewController alloc]initWithActivityItems: postItems applicationActivities: nil];
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [activityVc respondsToSelector: @selector(popoverPresentationController)])
    {
        UIPopoverController* popup = [[UIPopoverController alloc] initWithContentViewController: activityVc];
		UIView* view = [UIApplication sharedApplication].keyWindow.rootViewController.view;
        [popup presentPopoverFromRect: CGRectMake(view.frame.size.width/2, view.frame.size.height/4, 0, 0) inView: view permittedArrowDirections: UIPopoverArrowDirectionAny animated: YES];
    }
    else
    {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController: activityVc animated: YES completion: nil];
    }
}
