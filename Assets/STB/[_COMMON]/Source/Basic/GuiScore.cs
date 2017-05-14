using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// CLASS NAME: GuiScore
    /// # To handle 2D scoreboards using canvas
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GuiScore : GuiGeneric
    {
        // public
        public bool scaleAnimation = false;
        public bool inverseAnimation = false;

        // private
        Text textComponent = null;
        Vector3 baseScale = Vector3.zero;
        Generic.GenericSplineFunction scaleSpline = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StartExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void StartExtended()
        {
            textComponent = this.GetComponent<Text>();

            baseScale = textComponent.rectTransform.localScale;

            if (scaleAnimation)
            {
                scaleSpline = new Generic.GenericSplineFunction();
                scaleSpline.AddPoint(Vector3.one, false);

                if (inverseAnimation)
                {
                    scaleSpline.AddPoint(0.5f * Vector3.one, false);
                }
                else
                {
                    scaleSpline.AddPoint(1.5f * Vector3.one, false);
                }

                scaleSpline.AddPoint(Vector3.one, true);

                scaleSpline.ForceEnd();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetText
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetText(string t)
        {
            if (textComponent && (textComponent.text != t))
            {
                textComponent.text = t;

                RestartScaleAnimation();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetColor
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetColor(Color c)
        {
            textComponent.color = c;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RestartScaleAnimation
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RestartScaleAnimation()
        {
            if (scaleAnimation)
            {
                scaleSpline.Restart();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateFading
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateFading()
        {
            if (textComponent)
            {
                Color actualColor = textComponent.color;
                actualColor.a = Fading;
                textComponent.color = actualColor;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- OVERRRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void UpdateExtended()
        {
            if (scaleAnimation && (scaleSpline != null))
            {
                scaleSpline.UpdateUntilEnd(4000 * Time.deltaTime);

                textComponent.rectTransform.localScale = new Vector2(scaleSpline.GetActualPoint().x * baseScale.x, scaleSpline.GetActualPoint().y * baseScale.y);
            }

            // fading
            UpdateFading();
        }
    }
}