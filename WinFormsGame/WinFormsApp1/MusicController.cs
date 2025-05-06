using System;
using System.Media;
using System.IO;
using System.Windows.Forms;

public class MusicController
{
    #region �������������
    private SoundPlayer soundPlayer;
    private string musicFolderPath = "music";

    public MusicController(string musicFolder)
    {
        musicFolderPath = musicFolder;
        soundPlayer = new SoundPlayer();
    }
    #endregion

    #region ������ ��� ��������������� ������
    public void PlaySound(string fileName)
    {
        string filePath = Path.Combine(musicFolderPath, fileName);

        soundPlayer.SoundLocation = filePath;
        soundPlayer.Play();

    }

    public void PlayMusicLoop(string fileName)
    {
        string filePath = Path.Combine(musicFolderPath, fileName);

        soundPlayer.SoundLocation = filePath;
        soundPlayer.PlayLooping();
    }

    public void StopMusic()
    {
        soundPlayer.Stop();
    }
    #endregion
}