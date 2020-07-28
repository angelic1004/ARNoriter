#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

int saveImageToGallery(const char* path)
{
    NSString* imagePath = [NSString stringWithUTF8String: path];
    if ([[NSFileManager defaultManager] fileExistsAtPath: imagePath])
    {
        UIImage* image = [UIImage imageWithContentsOfFile: imagePath];
        if (image)
        {
            UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
            NSLog(@"Save image to gallery : %@", imagePath);
            return 1;
        }
        NSLog(@"Failed to save image to gallery : %@", imagePath);
        return -1;
    }
    NSLog(@"Image file not found : %@", imagePath);
    return 0;
}

int saveVideoToGallery(const char* path)
{
    NSString* videoPath = [NSString stringWithUTF8String: path];
    if ([[NSFileManager defaultManager] fileExistsAtPath: videoPath])
    {
        if (UIVideoAtPathIsCompatibleWithSavedPhotosAlbum(videoPath))
        {
            UISaveVideoAtPathToSavedPhotosAlbum(videoPath, nil, nil, nil);
            NSLog(@"Save video to gallery : %@", videoPath);
            return 1;
        }
        NSLog(@"Failed to save video to gallery : %@", videoPath);
        return -1;
    }
    NSLog(@"Video file not found : %@", videoPath);
    return 0;
}
