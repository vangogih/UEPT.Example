using System;
using DataSakura.AA.Runtime.Utilities.Logging;
using UnityEngine;

namespace DataSakura.AA.Runtime.Utilities
{
    public enum FitmentType
    {
        Left,
        Right,
        Top,
        Bottom,
        Horizontal,
        Vertical
    }

    public static class UISafeAreaFitter
    {
        public static void FitInSafeArea(this RectTransform trans)
        {
            Rect safeAreaRect = Screen.safeArea;

            //test safe area
            // Rect safeAreaRectRaw = Screen.safeArea;
            // const float offset = 50;
            // Rect safeAreaRect = new Rect(safeAreaRectRaw.x + offset, 
            //     safeAreaRectRaw.y + offset, 
            //     safeAreaRectRaw.width - offset * 2, 
            //     safeAreaRectRaw.height - offset * 2);

            Vector2 anchorMin = safeAreaRect.position;
            Vector2 anchorMax = safeAreaRect.position + safeAreaRect.size;

#if UNITY_IOS
            anchorMin.y /= 3;
#endif
            
            Log.Default.D("SAFE AREA",$"anchor min {anchorMin} max {anchorMax}");
            
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            trans.anchorMin = anchorMin;
            trans.anchorMax = anchorMax;

            Log.Default.D("SAFE AREA",
                $"applied to {trans.name}: x={safeAreaRect.x}, y={safeAreaRect.y}, w={safeAreaRect.width}, h={safeAreaRect.height} on full extents w={Screen.width}, h={Screen.height}");
        }

        public static void FitInSafeArea(this RectTransform trans, FitmentType fitmentType, float offset = 0)
        {
            Rect safeAreaRect = Screen.safeArea;

            if (safeAreaRect.height + 1 > Screen.height || safeAreaRect.width + 1 > Screen.width)
                return;

            safeAreaRect.height += (Screen.height - safeAreaRect.height) / 2;
            safeAreaRect.width += (Screen.width - safeAreaRect.width) / 2;

            if (fitmentType == FitmentType.Left || fitmentType == FitmentType.Right)
                safeAreaRect.width += offset;

            if (fitmentType == FitmentType.Top || fitmentType == FitmentType.Bottom)
                safeAreaRect.height += offset;

            Vector2 anchorMin = safeAreaRect.position;
            Vector2 anchorMax = safeAreaRect.position + safeAreaRect.size;

            switch (fitmentType) {
                case FitmentType.Left:
                    //anchorMin.x /= Screen.width;
                    anchorMin.x = 1 - safeAreaRect.width / Screen.width;

                    anchorMin.y = trans.anchorMin.y;
                    anchorMax.x = trans.anchorMax.x;
                    anchorMax.y = trans.anchorMax.y;
                    break;
                case FitmentType.Right:
                    //anchorMax.x /= Screen.width;
                    anchorMax.x = safeAreaRect.width / Screen.width;

                    anchorMin.x = trans.anchorMin.x;
                    anchorMin.y = trans.anchorMin.y;
                    anchorMax.y = trans.anchorMax.y;
                    break;
                case FitmentType.Top:
                    //anchorMax.y /= Screen.height;
                    anchorMax.y = safeAreaRect.height / Screen.height;

                    anchorMin.x = trans.anchorMin.x;
                    anchorMin.y = trans.anchorMin.y;
                    anchorMax.x = trans.anchorMax.x;
                    break;
                case FitmentType.Bottom:
                    //anchorMin.y /= Screen.height;
                    anchorMin.y = 1 - safeAreaRect.height / Screen.height;

                    anchorMin.x = trans.anchorMin.x;
                    anchorMax.y = trans.anchorMax.y;
                    anchorMax.x = trans.anchorMax.x;
                    break;
                case FitmentType.Horizontal:
                    anchorMin.x = 1 - safeAreaRect.width / Screen.width;
                    anchorMax.x = safeAreaRect.width / Screen.width;
                    anchorMin.y = trans.anchorMin.y;
                    anchorMax.y = trans.anchorMax.y;
                    break;
                case FitmentType.Vertical:
                    anchorMin.y = 1 - safeAreaRect.height / Screen.height;
                    anchorMax.y = safeAreaRect.height / Screen.height;
                    anchorMin.x = trans.anchorMin.x;
                    anchorMax.x = trans.anchorMax.x;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(fitmentType), fitmentType, null);
            }

            trans.anchorMin = anchorMin;
            trans.anchorMax = anchorMax;

            Log.Default.D("SAFE AREA",
                $"applied to {trans.name}: x={safeAreaRect.x}, y={safeAreaRect.y}, w={safeAreaRect.width}, h={safeAreaRect.height} on full extents w={Screen.width}, h={Screen.height}");
        }

        public static void MoveInSafeArea(this RectTransform trans, FitmentType fitmentType, float offset = 0)
        {
            Rect safeAreaRect = Screen.safeArea;

            if (safeAreaRect.height + 1 > Screen.height || safeAreaRect.width + 1 > Screen.width)
                return;

            safeAreaRect.height = (Screen.height - safeAreaRect.height) / 2;
            safeAreaRect.width = (Screen.width - safeAreaRect.width) / 2;

            if (fitmentType == FitmentType.Left || fitmentType == FitmentType.Right)
                safeAreaRect.width += offset;

            if (fitmentType == FitmentType.Top || fitmentType == FitmentType.Bottom)
                safeAreaRect.height += offset;

            Vector2 pos = trans.anchoredPosition;

            switch (fitmentType) {
                case FitmentType.Left:
                    pos.x -= safeAreaRect.width / 2 + offset;
                    break;
                case FitmentType.Right:
                    pos.x += safeAreaRect.width / 2 + offset;
                    break;
                case FitmentType.Top:
                    pos.y += safeAreaRect.height / 2 + offset;
                    break;
                case FitmentType.Bottom:
                    pos.y -= safeAreaRect.height / 2 + offset;
                    break;
            }

            trans.anchoredPosition = pos;

            Log.Default.D("SAFE AREA",
                $"applied to {trans.name}: x={safeAreaRect.x}, y={safeAreaRect.y}, w={safeAreaRect.width}, h={safeAreaRect.height} on full extents w={Screen.width}, h={Screen.height}");
        }
    }
}