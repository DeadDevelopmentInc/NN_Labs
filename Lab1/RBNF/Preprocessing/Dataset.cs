using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBNF.Preprocessing
{
    class Dataset
    {
        private string TrainingDataUpPath, TrainingDataDownPath, TrainingDataLeftPath, TrainingDataRightPath;
        private string TestingDataUpPath, TestingDataDownPath, TestingDataLeftPath, TestingDataRightPath;

        private int ClassNo = 4;
        private int TrainingNoInClass = 50;
        private int TestingNoInClass = 30;
        private int SamplesWidth = 50;
        private int SamplesHeight = 50;

        private byte[,,] TrainingInput;
        private byte[,,] TestingInput;

        public Dataset(string DataPath)
        {
            TrainingDataUpPath = DataPath + "Training/Data_Up/";
            TrainingDataLeftPath = DataPath + "Training/Data_Left/";
            TrainingDataRightPath = DataPath + "Training/Data_Right/";
            TrainingDataDownPath = DataPath + "Training/Data_Down/";

            TestingDataUpPath = DataPath + "Testing/Test_Up/";
            TestingDataDownPath = DataPath + "Testing/Test_Down/";
            TestingDataLeftPath = DataPath + "Testing/Test_Left/";
            TestingDataRightPath = DataPath + "Testing/Test_Right/";

            TrainingInput = new byte[ClassNo, TrainingNoInClass, SamplesHeight * SamplesWidth];
            TestingInput = new byte[ClassNo, TestingNoInClass, SamplesHeight * SamplesWidth];

            LoadTraining();
            LoadTesting();
        }

        public byte[,,] GetTrainingInput()
        {
            return TrainingInput;
        }
        public byte[,,] GetTestingInput()
        {
            return TestingInput;
        }

        private byte[] NNInput(byte[,] ImageBuffer)
        {
            int Height = ImageBuffer.GetLength(1);
            int Width = ImageBuffer.GetLength(0);
            byte[] Buffer = new byte[Width * Height];
            int Index = 0;
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    Buffer[Index++] = ImageBuffer[r, c];
                }
            }

            return Buffer;
        }

        private void FillTrainingInput(int ClassNumber, int ImageNumber, byte[] Buffer)
        {
            for (int j = 0; j < Buffer.Length; j++)
            {
                TrainingInput[ClassNumber - 1, ImageNumber - 1, j] = Buffer[j];
            }
        }

        private void FillTestingInput(int ClassNumber, int ImageNumber, byte[] Buffer)
        {
            for (int j = 0; j < Buffer.Length; j++)
            {
                TestingInput[ClassNumber - 1, ImageNumber - 1, j] = Buffer[j];
            }
        }

        private void LoadTraining()
        {
            Image OneImage = new Image();
            byte[] Buffer;
            for (int i = 1; i <= TrainingNoInClass; i++)
            {
                //Class #1
                Buffer = NNInput(OneImage.GetImage(TrainingDataUpPath + i.ToString() + ".png"));
                FillTrainingInput(1, i, Buffer);
                //Class #2
                Buffer = NNInput(OneImage.GetImage(TrainingDataLeftPath + i.ToString() + ".png"));
                FillTrainingInput(2, i, Buffer);
                //Class #3
                Buffer = NNInput(OneImage.GetImage(TrainingDataRightPath + i.ToString() + ".png"));
                FillTrainingInput(3, i, Buffer);
                //Class #4
                Buffer = NNInput(OneImage.GetImage(TrainingDataDownPath + i.ToString() + ".png"));
                FillTrainingInput(4, i, Buffer);
            }
        }

        private void LoadTesting()
        {
            Image OneImage = new Image();
            byte[] Buffer;
            for (int i = 1; i <= TestingNoInClass; i++)
            {
                //Class #1
                Buffer = NNInput(OneImage.GetImage(TestingDataUpPath +  i.ToString() + ".png"));
                FillTestingInput(1, i, Buffer);
                //Class #2
                Buffer = NNInput(OneImage.GetImage(TestingDataLeftPath +  i.ToString() + ".png"));
                FillTestingInput(2, i, Buffer);
                //Class #3
                Buffer = NNInput(OneImage.GetImage(TestingDataRightPath +  i.ToString() + ".png"));
                FillTestingInput(3, i, Buffer);
                //Class #4
                Buffer = NNInput(OneImage.GetImage(TestingDataDownPath + i.ToString() + ".png"));
                FillTestingInput(4, i, Buffer);
            }
        }
    }
}
