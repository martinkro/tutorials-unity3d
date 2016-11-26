using UnityEngine;
using System.Collections;
using Ionic.Zlib;
using System;
using System.IO;

public class BuildScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        FileStream input = new FileStream("E:/gamecheat/resource/ttlz/dump.txt", FileMode.Open, FileAccess.ReadWrite);
        FileStream output = new FileStream("E:/gamecheat/resource/ttlz/dump.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        string FileName_HashKey = "dump";
        long FileSize = input.Length;
        byte[] data = new byte[FileSize];
        input.Read(data, 0, (int)FileSize);
        System.Random random = new System.Random(FileName_HashKey.GetHashCode());
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= (byte)random.Next(256);
        }
        byte[] temp = ZlibStream.UncompressBuffer(data);
        for (int i = 0; i < temp.Length; i++)
        {
            output.WriteByte(temp[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
