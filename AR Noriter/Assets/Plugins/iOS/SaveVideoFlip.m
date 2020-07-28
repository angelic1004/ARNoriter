#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

int saveVideoVerticalFlip(const char* path1, const char* path2)
{
    NSString* videoPath = [NSString stringWithUTF8String: path1];
    NSString* outputPath = [NSString stringWithUTF8String: path2];

    NSURL* videoURL = [NSURL fileURLWithPath: videoPath];
    AVURLAsset *asset = [AVURLAsset URLAssetWithURL: videoURL options: @{ AVURLAssetPreferPreciseDurationAndTimingKey: @YES }];

    AVMutableVideoCompositionInstruction *instruction = nil;
    AVMutableVideoCompositionLayerInstruction *layerInstruction = nil;

    AVAssetTrack *assetVideoTrack = nil;
    AVAssetTrack *assetAudioTrack = nil;

    if ([[asset tracksWithMediaType: AVMediaTypeVideo] count] != 0) {
        assetVideoTrack = [asset tracksWithMediaType: AVMediaTypeVideo][0];
    }
    if ([[asset tracksWithMediaType: AVMediaTypeAudio] count] != 0) {
        assetAudioTrack = [asset tracksWithMediaType: AVMediaTypeAudio][0];
    }

    NSError *error = nil;
    AVMutableComposition *mutableComposition = [AVMutableComposition composition];

    if (assetVideoTrack != nil) {
        AVMutableCompositionTrack *compositionVideoTrack = [mutableComposition addMutableTrackWithMediaType: AVMediaTypeVideo preferredTrackID: kCMPersistentTrackID_Invalid];
        [compositionVideoTrack insertTimeRange: CMTimeRangeMake(kCMTimeZero, [asset duration]) ofTrack: assetVideoTrack atTime: kCMTimeZero error: &error];
    }
    if (assetAudioTrack != nil) {
        AVMutableCompositionTrack *compositionAudioTrack = [mutableComposition addMutableTrackWithMediaType: AVMediaTypeAudio preferredTrackID: kCMPersistentTrackID_Invalid];
        [compositionAudioTrack insertTimeRange: CMTimeRangeMake(kCMTimeZero, [asset duration]) ofTrack: assetAudioTrack atTime: kCMTimeZero error: &error];
    }

    if (error)
    {
        NSLog(@"Cannot read video file : %@", videoPath);
        return -1;
    }

    CGAffineTransform transform = CGAffineTransformMakeTranslation(0, assetVideoTrack.naturalSize.height);
    transform = CGAffineTransformScale(transform, 1, -1);

    AVMutableVideoComposition *mutableVideoComposition = [AVMutableVideoComposition videoComposition];
    mutableVideoComposition.renderSize = CGSizeMake(assetVideoTrack.naturalSize.width, assetVideoTrack.naturalSize.height);
    mutableVideoComposition.frameDuration = CMTimeMake(1, 30);

    instruction = [AVMutableVideoCompositionInstruction videoCompositionInstruction];
    instruction.timeRange = CMTimeRangeMake(kCMTimeZero, [mutableComposition duration]);
    layerInstruction = [AVMutableVideoCompositionLayerInstruction videoCompositionLayerInstructionWithAssetTrack: (mutableComposition.tracks)[0]];
    [layerInstruction setTransform: transform atTime: kCMTimeZero];

    instruction.layerInstructions = @[layerInstruction];
    mutableVideoComposition.instructions = @[instruction];

    AVAssetExportSession *exportSession = [[AVAssetExportSession alloc] initWithAsset: [mutableComposition copy] presetName: AVAssetExportPresetHighestQuality];
    exportSession.videoComposition = mutableVideoComposition;
    exportSession.outputURL = [NSURL fileURLWithPath: outputPath];
    exportSession.outputFileType = AVFileTypeMPEG4;
    exportSession.shouldOptimizeForNetworkUse = YES;

    static int result = 1;
    dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
    [exportSession exportAsynchronouslyWithCompletionHandler:
        ^ void {
            AVAssetExportSessionStatus status = exportSession.status;
            if (status != AVAssetExportSessionStatusCompleted) {
                NSLog(@"Cannot write video file : %@", outputPath);
                result = 0;
            }
            dispatch_semaphore_signal(semaphore);
        }
    ];
    dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
    return result;
}

static int completionState = -1;

int getSaveVideoCompletionState()
{
    return completionState;
}

int saveVideoVerticalFlipAsync(const char* path1, const char* path2)
{
    completionState = -1;

    NSString* videoPath = [NSString stringWithUTF8String: path1];
    NSString* outputPath = [NSString stringWithUTF8String: path2];

    NSURL* videoURL = [NSURL fileURLWithPath: videoPath];
    AVURLAsset *asset = [AVURLAsset URLAssetWithURL: videoURL options: @{ AVURLAssetPreferPreciseDurationAndTimingKey: @YES }];

    AVMutableVideoCompositionInstruction *instruction = nil;
    AVMutableVideoCompositionLayerInstruction *layerInstruction = nil;

    AVAssetTrack *assetVideoTrack = nil;
    AVAssetTrack *assetAudioTrack = nil;

    if ([[asset tracksWithMediaType: AVMediaTypeVideo] count] != 0) {
        assetVideoTrack = [asset tracksWithMediaType: AVMediaTypeVideo][0];
    }
    if ([[asset tracksWithMediaType: AVMediaTypeAudio] count] != 0) {
        assetAudioTrack = [asset tracksWithMediaType: AVMediaTypeAudio][0];
    }

    NSError *error = nil;
    AVMutableComposition *mutableComposition = [AVMutableComposition composition];

    if (assetVideoTrack != nil) {
        AVMutableCompositionTrack *compositionVideoTrack = [mutableComposition addMutableTrackWithMediaType: AVMediaTypeVideo preferredTrackID: kCMPersistentTrackID_Invalid];
        [compositionVideoTrack insertTimeRange: CMTimeRangeMake(kCMTimeZero, [asset duration]) ofTrack: assetVideoTrack atTime: kCMTimeZero error: &error];
    }
    if (assetAudioTrack != nil) {
        AVMutableCompositionTrack *compositionAudioTrack = [mutableComposition addMutableTrackWithMediaType: AVMediaTypeAudio preferredTrackID: kCMPersistentTrackID_Invalid];
        [compositionAudioTrack insertTimeRange: CMTimeRangeMake(kCMTimeZero, [asset duration]) ofTrack: assetAudioTrack atTime: kCMTimeZero error: &error];
    }

    if (error)
    {
        NSLog(@"Cannot read video file : %@", videoPath);
        return -1;
    }

    CGAffineTransform transform = CGAffineTransformMakeTranslation(0, assetVideoTrack.naturalSize.height);
    transform = CGAffineTransformScale(transform, 1, -1);

    AVMutableVideoComposition *mutableVideoComposition = [AVMutableVideoComposition videoComposition];
    mutableVideoComposition.renderSize = CGSizeMake(assetVideoTrack.naturalSize.width, assetVideoTrack.naturalSize.height);
    mutableVideoComposition.frameDuration = CMTimeMake(1, 30);

    instruction = [AVMutableVideoCompositionInstruction videoCompositionInstruction];
    instruction.timeRange = CMTimeRangeMake(kCMTimeZero, [mutableComposition duration]);
    layerInstruction = [AVMutableVideoCompositionLayerInstruction videoCompositionLayerInstructionWithAssetTrack: (mutableComposition.tracks)[0]];
    [layerInstruction setTransform: transform atTime: kCMTimeZero];

    instruction.layerInstructions = @[layerInstruction];
    mutableVideoComposition.instructions = @[instruction];

    AVAssetExportSession *exportSession = [[AVAssetExportSession alloc] initWithAsset: [mutableComposition copy] presetName: AVAssetExportPresetHighestQuality];
    exportSession.videoComposition = mutableVideoComposition;
    exportSession.outputURL = [NSURL fileURLWithPath: outputPath];
    exportSession.outputFileType = AVFileTypeMPEG4;
    exportSession.shouldOptimizeForNetworkUse = YES;

    completionState = 0;
    [exportSession exportAsynchronouslyWithCompletionHandler:
        ^ void {
            AVAssetExportSessionStatus status = exportSession.status;
            if (status == AVAssetExportSessionStatusCompleted) {
                completionState = 1;
            }
			else {
                completionState = -1;
            }
        }
    ];
	return 1;
}
