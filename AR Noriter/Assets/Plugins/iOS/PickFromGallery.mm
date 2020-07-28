#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <MobileCoreServices/UTCoreTypes.h>

@interface NativeGallery:NSObject
+ (void)pickFromGallery:(int)pickType;
@end

@implementation NativeGallery

const int PICK_IMAGE = 1;
const int PICK_AUDIO = 2;
const int PICK_VIDEO = 3;
const int PICK_IMAGE_OR_VIDEO = 4;

static NSString *targetTransform;
static NSString *targetEventFunc;
static UIPopoverController *popup;

+ (void)pickFromGallery:(int)pickType
{
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.allowsEditing = NO;

    if (pickType == PICK_IMAGE) {
        picker.mediaTypes = [NSArray arrayWithObject:(NSString *)kUTTypeImage];
    }
    else if (pickType == PICK_VIDEO) {
        picker.mediaTypes = [NSArray arrayWithObjects:(NSString *)kUTTypeMovie, (NSString *)kUTTypeVideo, nil];
    }
    else if (pickType == PICK_AUDIO) {
        // Cannot pick audio with UIImagePickerController
        //picker.mediaTypes = [NSArray arrayWithObject:(NSString *)kUTTypeAudio];
        NSLog(@"Unsupported pick type : %d", pickType);
        return;
    }
    else if (pickType == PICK_IMAGE_OR_VIDEO) {
        picker.mediaTypes = [NSArray arrayWithObjects:(NSString *)kUTTypeImage, (NSString *)kUTTypeMovie, (NSString *)kUTTypeVideo, nil];
    }

    if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary]) {
        NSLog(@"Pick from photo library : %d", pickType);
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    }
    else if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum]) {
        NSLog(@"Pick from saved photos album : %d", pickType);
        picker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    }
    else {
        NSLog(@"Pick is not available");
        UnitySendMessage([targetTransform UTF8String], [targetEventFunc UTF8String], "");
        return;
    }

    UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) // iPhone
    {
        [rootViewController presentViewController:picker animated:YES completion:nil];
    }
    else // iPad
    {
        popup = [[UIPopoverController alloc] initWithContentViewController:picker];
        popup.delegate = self;
        [popup presentPopoverFromRect:CGRectMake( rootViewController.view.frame.size.width / 2, rootViewController.view.frame.size.height / 4, 0, 0 ) inView:rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
}

+ (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    NSString *path;
    if (info[UIImagePickerControllerMediaType] == (NSString *)kUTTypeImage) // image
    {
        UIImage *image = [info objectForKey:UIImagePickerControllerOriginalImage];
        if (image != nil)
        {
            NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
            NSString *docsDir = [paths objectAtIndex:0];
            NSString *imagePath = [docsDir stringByAppendingPathComponent:@"selected.png"];
            NSData *imageData = UIImagePNGRepresentation(image);
            [imageData writeToFile:imagePath atomically:YES];
            path = imagePath;
        }
    }
    else // video or other media
    {
        NSURL *mediaUrl = info[UIImagePickerControllerMediaURL] ?: info[UIImagePickerControllerReferenceURL];
        if (mediaUrl == nil)
            path = nil;
        else
            path = [mediaUrl path];
    }

    if (path == nil)
        path = @"";

    const char *pathUTF8 = [path UTF8String];
    char *result = (char*) malloc(strlen(pathUTF8) + 1);
    strcpy(result, pathUTF8);

    popup = nil;
    UnitySendMessage([targetTransform UTF8String], [targetEventFunc UTF8String], result);

    [picker dismissViewControllerAnimated:YES completion:nil];
}

+ (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    popup = nil;
    UnitySendMessage([targetTransform UTF8String], [targetEventFunc UTF8String], "");

    [picker dismissViewControllerAnimated:YES completion:nil];
}

+ (void)popoverControllerDidDismissPopover:(UIPopoverController *)popoverController
{
    popup = nil;
    UnitySendMessage([targetTransform UTF8String], [targetEventFunc UTF8String], "");
}

@end

extern "C" void pickFromGallery(const char* transform, const char* eventFunc, int pickType)
{
    targetTransform = [NSString stringWithUTF8String:transform];
    targetEventFunc = [NSString stringWithUTF8String:eventFunc];
    [NativeGallery pickFromGallery:pickType];
}

extern "C" void pickImageFromGallery(const char* transform, const char* eventFunc)
{
    targetTransform = [NSString stringWithUTF8String:transform];
    targetEventFunc = [NSString stringWithUTF8String:eventFunc];
    [NativeGallery pickFromGallery:PICK_IMAGE];
}

extern "C" void pickVideoFromGallery(const char* transform, const char* eventFunc)
{
    targetTransform = [NSString stringWithUTF8String:transform];
    targetEventFunc = [NSString stringWithUTF8String:eventFunc];
    [NativeGallery pickFromGallery:PICK_VIDEO];
}

extern "C" void pickImageOrVideoFromGallery(const char* transform, const char* eventFunc)
{
    targetTransform = [NSString stringWithUTF8String:transform];
    targetEventFunc = [NSString stringWithUTF8String:eventFunc];
    [NativeGallery pickFromGallery:PICK_IMAGE_OR_VIDEO];
}
