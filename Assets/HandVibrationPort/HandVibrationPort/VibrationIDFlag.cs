using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expect.Vibration
{
    public class VibrationIDFlag
    {

        public class RightHand
        {
            public const string CodeThumb = "00:1";
            public const string CodeIndexFinger = "00:2";
            public const string CodeMiddleFinger = "01:0";
            public const string CodeRingFinger = "01:1";
            public const string CodeBabyFinger = "01:2";
            public const string CodeCenter = "00:0";
        }

        public class LeftHand
        {
            public const string CodeThumb = "00:1";
            public const string CodeIndexFinger = "01:2";
            public const string CodeMiddleFinger = "01:1";
            public const string CodeRingFinger = "01:0";
            public const string CodeBabyFinger = "00:2";
            public const string CodeCenter = "00:0";
        }

        public class GeneralHand
        {
            public const string H_FullHandVibration = "01111111";
            public const string L_FullHandVibration = "00111111";
        }

        public class VibrationPower {
            public const string None = "00";
            public const string Weak = "01";
            public const string Normal = "10";
            public const string Strong = "11";
        }

        public class VibrationHeader
        {
            public const string H = "01";
            public const string L = "00";
        }

        public enum FingerIndex {
            //大拇指
            Thumb,
            //食指
            IndexFinger,
            //中指
            MiddleFinger,
            //無名指
            RingFinger,
            //小指頭
            BabyFinger,
            //手掌
            Center
        }

        public enum HandIndex
        {
            Right, Left
        }

    }
}