 
//cs example
//capture touch events and draw vectors connecting them
//assume touch define a circle, so measure variation in radius and compute centre
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class Lines : MonoBehaviour {
 
    private static readonly Color lineColor  = new Color (1f, 0f, 0f, 0.88f);
    private static readonly Color lineColorLight  = new Color (1f, 0f, 0f, 0.28f);
    private static bool done = false;
    private static Vector2 vCenter;
    private static Vector2 vRadius;
    private static float fAngle;
    public GameController gameController;
   
    List<Vector2> lineList;
   
    // Use this for initialization
    void Start ()
    {
        CreateLineMaterial();
        lineList = new List<Vector2>();
    }
 
    void Update()
    {
        Vector2 tmp;
#if UNITY_IPHONE
        if (Input.touchCount > 0)
        {
            tmp = Input.GetTouch(0).position;
            tmp.y = Screen.height - tmp.y; // touch starts top left
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                gameController.levelEvent = "cross";
                lineList.Clear();
                done = false;
            }
            lineList.Add(tmp);
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                done = true;
                StartCoroutine(resetLevelEvent());
                ComputeStats();
            }          
        }
#endif
#if UNITY_EDITOR
        tmp = Input.mousePosition;
        tmp.y = Screen.height - tmp.y; // touch starts top left
        if (Input.GetMouseButtonDown(0))
        {
            lineList.Clear();
            done = false;
            lineList.Add(tmp);
            gameController.levelEvent = "cross";
        }
        if (Input.GetMouseButtonUp(0))
        {
            lineList.Add(tmp);
            done = true;
            ComputeStats();
        }
        if (Input.GetMouseButton(0))
        {
            StartCoroutine(resetLevelEvent());
            lineList.Add(tmp);
        }
#endif
    }
       
    void OnGUI()
    {
        DrawList();
       
        if (done)
        {
            GUI.Label(new Rect(10, 10, 500, 25), "Center: " + vCenter.ToString());
            GUI.Label(new Rect(10, 30, 500, 25), "Radius: " + vRadius.x + ".." + vRadius.y);
            GUI.Label(new Rect(10, 50, 500, 25), "Angle: " + fAngle);
        }
    }
   
    void DrawList ()
    {
        if (Event.current.type != EventType.Repaint)
            return;
 
        lineMaterial.SetPass(0);
       
        GL.PushMatrix ();
         GL.Begin (GL.LINES);
       
            DrawLines ();
 
         GL.End ();
        GL.PopMatrix ();
       
    }
 
    void DrawLines()
    {
        if (lineList.Count > 1)
        {
            vCenter = lineList[0];
           
            // draw outline
            GL.Color (lineColor);
            for (int i = 1; i < lineList.Count; i++)
            {
                DrawLine (lineList[i-1], lineList[i]);
                vCenter += lineList[i];
            }
 
            vCenter /= lineList.Count;
           
            // draw spokes
            GL.Color (lineColorLight);
            for (int i = 0; i < lineList.Count; i++)
            {
                DrawLine (vCenter, lineList[i]);
            }
        }
    }
 
    private void DrawLine (Vector2 p1, Vector2 p2)
    {
        GL.Vertex (p1);
        GL.Vertex (p2);
    }
 
    private void ComputeStats()
    {
        if (lineList.Count > 1)
        {
            // compute center
            vCenter = Vector2.zero;
            for (int i = 0; i < lineList.Count; i++)
            {
                vCenter += lineList[i];
            }
 
            vCenter /= lineList.Count;
 
            // get min and max radius
            vRadius = Vector2.zero;
            vRadius.x = 123456.0f;
           
            Vector2 tmp;
            float len;
            for (int i = 0; i < lineList.Count; i++)
            {
                tmp = lineList[i] - vCenter;
                len = tmp.magnitude;
               
                vRadius.x = (len < vRadius.x) ? len : vRadius.x;
                vRadius.y = (len > vRadius.y) ? len : vRadius.y;
            }
           
            // use angles to work out completeness
            // and to decide if it's a circle at all... hmm, thinks...
           
            Vector2 a = lineList[0] - vCenter;
            fAngle = 0;
            for (int i = 1; i < lineList.Count; i++)
            {
                Vector2 b = lineList[i] - vCenter;
               
                fAngle += Vector2.Angle(a,b);
                a = b;             
            }
        }
    }
       
    private Material lineMaterial;
   
    private void CreateLineMaterial()
    {
        if( !lineMaterial ) {
            lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }" );
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    IEnumerator resetLevelEvent() {
        yield return new WaitForSeconds(6);
        if(gameController.levelEvent == "cross") {
            gameController.levelEvent = "standingStill";
        }
        yield return null;
    }
}