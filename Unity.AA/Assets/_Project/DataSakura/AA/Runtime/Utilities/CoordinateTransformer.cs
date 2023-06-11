using UnityEngine;

namespace DataSakura.Runtime.Utilities
{
    public static class CoordinateTransformer
    {
        /// <summary>
        /// Находит ЛОКАЛЬНУЮ позицию для объекта в канвасе. Он должен быть непосредственным chield-ом канваса
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="camera"></param>
        /// <param name="canvasRectTransform"></param>
        /// <returns></returns>
        public static Vector2 FromWorldToCanvasLocalPos(Vector3 worldPos, Camera camera, RectTransform canvasRectTransform)
        {
            Vector2 screenPos = camera.WorldToViewportPoint(worldPos);
            screenPos.x -= 0.5f;
            screenPos.y -= 0.5f;
            var rect = canvasRectTransform.rect;
            screenPos.x *= rect.width;
            screenPos.y *= rect.height;
            return screenPos;
        }

        /// <summary>
        /// Находит ЛОКАЛЬНУЮ позицию для объекта в канвасе. Он должен быть непосредственным chield-ом канваса
        /// </summary>
        /// <param name="screenPos">Позиция экрана (не путать с позицией в канвасе)</param>
        /// <param name="camera"></param>
        /// <param name="canvasRectTransform"></param>
        /// <returns></returns>
        public static Vector2 FromScreenToCanvasLocalPos(Vector2 screenPos, Camera camera, RectTransform canvasRectTransform)
        {
            Vector2 canvasPos = camera.ScreenToViewportPoint(screenPos);
            canvasPos.x -= 0.5f;
            canvasPos.y -= 0.5f;
            var rect = canvasRectTransform.rect;
            canvasPos.x *= rect.width;
            canvasPos.y *= rect.height;
            return canvasPos;
        }

        public static Vector2 FromWorldToCanvasGlobalPos(Vector3 worldPos, Camera camera, RectTransform canvasRectTransform)
        {
            Vector2 screenPos = camera.WorldToViewportPoint(worldPos);
            screenPos.x -= 0.5f;
            screenPos.y -= 0.5f;
            var rect = canvasRectTransform.rect;
            var localScale = canvasRectTransform.localScale;
            screenPos.x *= rect.width * localScale.x;
            screenPos.y *= rect.height * localScale.y;
            return screenPos;
        }

        public static Vector2 FromScreenToCanvasGlobalPos(Vector2 screenPos, Camera camera, RectTransform canvasRectTransform)
        {
            Vector2 canvasPos = camera.ScreenToViewportPoint(screenPos);
            canvasPos.x -= 0.5f;
            canvasPos.y -= 0.5f;
            var rect = canvasRectTransform.rect;
            var localScale = canvasRectTransform.localScale;
            canvasPos.x *= rect.width * localScale.x;
            canvasPos.y *= rect.height * localScale.y;
            return canvasPos;
        }
    }
}