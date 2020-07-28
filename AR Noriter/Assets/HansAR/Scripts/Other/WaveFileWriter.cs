using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

/// <summary>
/// 오디오 클립을 웨이브 파일로 저장하는 클래스
/// </summary>
public class WaveFileWriter
{
    /// <summary>
    /// 헤더의 크기
    /// </summary>
    private const int HEADER_SIZE = 44;
    /// <summary>
    /// 샘플당 바이트 수
    /// </summary>
    private const int BYTES_PER_SAMPLE = 2;

    /// <summary>
    /// 오디오 클립을 웨이브 파일로 저장합니다. 실제 녹음한 시간만큼만 저장하기 위해 샘플링 수를 설정합니다.
    /// </summary>
    /// <param name="filePath">저장할 파일명</param>
    /// <param name="clip">저장할 오디오 클립</param>
    /// <param name="samples">샘플링 수</param>
    /// <returns></returns>
    public static bool Save(string filePath, AudioClip clip, int samples)
    {
        bool result = false;

        // 샘플링 수가 잘못된 경우 오디오 클립의 샘플링 수를 사용한다.
        if (samples < 1)
        {
            samples = clip.samples;
        }

        // 디렉토리가 존재하지 않으면 생성한다.
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // 파일이 이미 존재하면 삭제한다.
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        MemoryStream stream = new MemoryStream();
        try
        {
            // 헤더를 기록한다.
            WriteHeader(stream, clip, samples);

            // 데이터를 기록한다.
            WriteData(stream, clip, samples);

            // 메모리스트림의 내용을 파일에 저장한다.
            File.WriteAllBytes(filePath, stream.GetBuffer());
            result = true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        stream.Close();
        stream.Dispose();

        // 기록에 실패하였으면 파일을 삭제한다.
        if (!result)
        {
            File.Delete(filePath);
        }
        return result;
    }

    /// <summary>
    /// 스트림에 바이트 배열을 기록합니다.
    /// </summary>
    /// <param name="stream">스트림</param>
    /// <param name="bytes">바이트 배열</param>
    private static void WriteBytes(Stream stream, byte[] bytes)
    {
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// 웨이브 파일의 헤더를 기록합니다.
    /// </summary>
    /// <param name="stream">스트림</param>
    /// <param name="clip">오디오 클립</param>
    /// <param name="samples">샘플링 수</param>
    private static void WriteHeader(Stream stream, AudioClip clip, int samples)
    {
        int dataSize = samples * clip.channels * BYTES_PER_SAMPLE;

        // "RIFF" header (4 바이트)
        WriteBytes(stream, Encoding.UTF8.GetBytes("RIFF"));

        // chunk size (4 바이트)
        WriteBytes(stream, BitConverter.GetBytes((uint)(HEADER_SIZE + dataSize - 8))); // "RIFF" header와 chunk size를 제외한 전체 파일 크기

        // "WAVE" header (4 바이트)
        WriteBytes(stream, Encoding.UTF8.GetBytes("WAVE"));

        // "fmt " section (4 바이트)
        WriteBytes(stream, Encoding.UTF8.GetBytes("fmt "));

        // chunk 1 (4 바이트)
        WriteBytes(stream, BitConverter.GetBytes((uint)(16))); // 16 : audio format에서 bits per sample까지의 크기

        // audio format (2 바이트)
        WriteBytes(stream, BitConverter.GetBytes((ushort)(1))); // 1 : uncompressed PCM

        // number of channels (2 바이트)
        WriteBytes(stream, BitConverter.GetBytes((ushort)(clip.channels)));

        // sample rate (4 바이트)
        WriteBytes(stream, BitConverter.GetBytes((uint)(clip.frequency)));

        // byte rate (4 바이트)
        WriteBytes(stream, BitConverter.GetBytes((uint)(clip.channels * clip.frequency * BYTES_PER_SAMPLE))); // 채널 수 * 샘플

        // block align (2 바이트)
        WriteBytes(stream, BitConverter.GetBytes((ushort)(clip.channels * BYTES_PER_SAMPLE)));

        // bits per sample (2 바이트)
        WriteBytes(stream, BitConverter.GetBytes((ushort)(BYTES_PER_SAMPLE * 8))); // bits = bytes * 8

        // "data" section (4 바이트)
        WriteBytes(stream, Encoding.UTF8.GetBytes("data"));

        // chunk 2 (4 바이트)
        WriteBytes(stream, BitConverter.GetBytes((uint)(dataSize)));
    }

    /// <summary>
    /// 웨이브 파일의 데이터를 기록합니다.
    /// </summary>
    /// <param name="stream">스트림</param>
    /// <param name="clip">오디오 클립</param>
    /// <param name="samples">샘플링 수</param>
    private static void WriteData(Stream stream, AudioClip clip, int samples)
    {
        // 오디오 클립의 데이터를 얻는다.
        float[] sampleData = new float[clip.samples * clip.channels];
        clip.GetData(sampleData, 0);

        // float 배열의 데이터를 byte 배열로 변환해서 기록한다.
        for (int i = 0; i < sampleData.Length; i++)
        {
            // 지정한 샘플링 수만큼만 기록하고 뒷부분은 버린다.
            if (i > samples * clip.channels)
            {
                // 헤더를 다시 기록할 필요가 없다. (일부러 주석처리함)
                //stream.Seek(0, SeekOrigin.Begin);
                //WriteHeader(stream, clip, (i - 1) * clip.channels);
                break;
            }

            // 데이터를 변환해서 기록한다.
            byte[] bytes = BitConverter.GetBytes((short)(sampleData[i] * short.MaxValue));
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
