using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace QuickMonsterCreditFix
{
    class Test : MonoBehaviour
    {

        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            int width = source.width * 2;
            int height = source.height * 2;
            RenderTextureFormat format = source.format;

            RenderTexture r = RenderTexture.GetTemporary(width, height, 0, format);

            r.filterMode = FilterMode.Point;

            Graphics.Blit(source, r);

            Graphics.Blit(r, destination);
            RenderTexture.ReleaseTemporary(r);
        }
    }
}
