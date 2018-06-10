using UnityEngine;

namespace ProceduralToolkit
{
    public static class MeshConstructorExtension
    {
        public static MeshDraft AddHexaheron(this MeshDraft draft, Vector3 origin, Vector3 width, Vector3 length,
            Vector3 height, bool generateUV = true)
        {
            Vector3 v000 = origin - width / 2 - length / 2 - height / 2;
            Vector3 v001 = v000 + height;
            Vector3 v010 = v000 + width;
            Vector3 v011 = v000 + width + height;
            Vector3 v100 = v000 + length;
            Vector3 v101 = v000 + length + height;
            Vector3 v110 = v000 + width + length;
            Vector3 v111 = v000 + width + length + height;

            if (generateUV)
            {
                Vector2 uv0 = new Vector2(0, 0);
                Vector2 uv1 = new Vector2(0, 1);
                Vector2 uv2 = new Vector2(1, 1);
                Vector2 uv3 = new Vector2(1, 0);
                draft.AddQuad(v100, v101, v001, v000, Vector3.left, uv0, uv1, uv2, uv3)
                    .AddQuad(v010, v011, v111, v110, Vector3.right, uv0, uv1, uv2, uv3)
                    .AddQuad(v010, v110, v100, v000, Vector3.down, uv0, uv1, uv2, uv3)
                    .AddQuad(v111, v011, v001, v101, Vector3.up, uv0, uv1, uv2, uv3)
                    .AddQuad(v000, v001, v011, v010, Vector3.back, uv0, uv1, uv2, uv3)
                    .AddQuad(v110, v111, v101, v100, Vector3.forward, uv0, uv1, uv2, uv3);
            }
            else
            {
                draft.AddQuad(v100, v101, v001, v000, Vector3.left)
                    .AddQuad(v010, v011, v111, v110, Vector3.right)
                    .AddQuad(v010, v110, v100, v000, Vector3.down)
                    .AddQuad(v111, v011, v001, v101, Vector3.up)
                    .AddQuad(v000, v001, v011, v010, Vector3.back)
                    .AddQuad(v110, v111, v101, v100, Vector3.forward);
            }

            return draft;
        }

        //assume pointA and pointB is on side-face
        public static MeshDraft AddSkewBrige(this MeshDraft draft, Vector3 pointA, Vector3 pointB, float w,
            float h, bool generateUV = true)
        {
            var isHorizetal = Mathf.Abs(pointA.z - pointB.z) < 0.001f;
            var width = isHorizetal ? new Vector3(0, 0, w) : new Vector3(w, 0, 0);
            var height = new Vector3(0, h, 0);
            Vector3 v000, v001, v010, v011, v100, v101, v110, v111;

            v000 = pointA - width / 2 - height / 2;
            v001 = v000 + height;
            v010 = v000 + width;
            v011 = v000 + height + width;
            v100 = pointB - width / 2 - height / 2;
            v101 = v100 + height;
            v110 = v100 + width;
            v111 = v100 + height + width;

            if (generateUV)
            {
                Vector2 uv0 = new Vector2(0, 0);
                Vector2 uv1 = new Vector2(0, 1);
                Vector2 uv2 = new Vector2(1, 1);
                Vector2 uv3 = new Vector2(1, 0);
                if (!isHorizetal)
                {
                    draft.AddQuad(v100, v101, v001, v000, Vector3.left, uv0, uv1, uv2, uv3)
                        .AddQuad(v010, v011, v111, v110, Vector3.right, uv0, uv1, uv2, uv3)
                        .AddQuad(v010, v110, v100, v000, Vector3.down, uv0, uv1, uv2, uv3)
                        .AddQuad(v111, v011, v001, v101, Vector3.up, uv0, uv1, uv2, uv3)
                        .AddQuad(v000, v001, v011, v010, Vector3.back, uv0, uv1, uv2, uv3)
                        .AddQuad(v110, v111, v101, v100, Vector3.forward, uv0, uv1, uv2, uv3);
                }
                else
                {
                    draft.AddQuad(v010, v011, v001, v000, Vector3.left, uv0, uv1, uv2, uv3)
                        .AddQuad(v100, v101, v111, v110, Vector3.right, uv0, uv1, uv2, uv3)
                        .AddQuad(v100, v110, v010, v000, Vector3.down, uv0, uv1, uv2, uv3)
                        .AddQuad(v111, v101, v001, v011, Vector3.up, uv0, uv1, uv2, uv3)
                        .AddQuad(v000, v001, v101, v100, Vector3.back, uv0, uv1, uv2, uv3)
                        .AddQuad(v110, v111, v011, v010, Vector3.forward, uv0, uv1, uv2, uv3); 
                }
            }
            else
            {
                if (!isHorizetal)
                {
                    draft.AddQuad(v100, v101, v001, v000, Vector3.left)
                        .AddQuad(v010, v011, v111, v110, Vector3.right)
                        .AddQuad(v010, v110, v100, v000, Vector3.down)
                        .AddQuad(v111, v011, v001, v101, Vector3.up)
                        .AddQuad(v000, v001, v011, v010, Vector3.back)
                        .AddQuad(v110, v111, v101, v100, Vector3.forward);
                }
                else
                {
                    draft.AddQuad(v010, v011, v001, v000, Vector3.left)
                        .AddQuad(v100, v101, v111, v110, Vector3.right)
                        .AddQuad(v100, v110, v010, v000, Vector3.down)
                        .AddQuad(v111, v101, v001, v011, Vector3.up)
                        .AddQuad(v000, v001, v101, v100, Vector3.back)
                        .AddQuad(v110, v111, v011, v010, Vector3.forward); 
                }
            }
            

            return draft;
        }
    }
}