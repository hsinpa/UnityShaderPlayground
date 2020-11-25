using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Expect.Vibration { 
    public class VibrationSampleCode : MonoBehaviour
    {

        public VibrationIDFlag.HandIndex handIndex;
        public VibrationIDFlag.FingerIndex[] fingerHandIndexes;

        public PowerEnum powerStrength;

        public enum PowerEnum {
            None, Weak, Normal, Strong
        }

        public void TestFullVibration() {
            string power = PowerMapper(powerStrength);
            VibrationHandler.instance.FullHandVibration(handIndex, power);
        }

        public void TestFingerVibration()
        {

            if (fingerHandIndexes != null) {
                string power = PowerMapper(powerStrength);
                VibrationHandler.instance.FingerVibration(handIndex, power, fingerHandIndexes);
            }
        }

        public string PowerMapper(PowerEnum powerEnum) {
            switch (powerEnum) {

                case PowerEnum.None:
                    return VibrationIDFlag.VibrationPower.None;

                case PowerEnum.Weak:
                    return VibrationIDFlag.VibrationPower.Weak;

                case PowerEnum.Normal:
                    return VibrationIDFlag.VibrationPower.Normal;

                case PowerEnum.Strong:
                    return VibrationIDFlag.VibrationPower.Strong;
            }

            return VibrationIDFlag.VibrationPower.None;
        }
    }
}