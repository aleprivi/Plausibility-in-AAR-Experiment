using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectPlane : MonoBehaviour
{
    public GameObject ObjectToFollow;
    public GameObject ArrowObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(ObjectToFollow.transform.position.x, gameObject.transform.position.y, ObjectToFollow.transform.position.z);
        TestRotate();
    }

    int rotation = 0;

    int old_x = 0;
    int old_y = 0;

    public void TestRotate()
    {
        int x = Mathf.RoundToInt(gameObject.transform.position.x * 2) * -1;
        int y = Mathf.RoundToInt(gameObject.transform.position.z * 2);
//        Debug.Log(x + "-" + y);



        if (((y > old_y) && (x%2 == 0)) || ((y < old_y) && (x % 2 == 1)) || (x > old_x))
        {

            bool HeadNotVisible = true;
            while (HeadNotVisible)
            {

                rotation += 90;
                rotation = rotation % 360;
                Debug.Log("rotation = " + rotation);

                transform.Rotate(new Vector3(0, 0, 90));




                switch (rotation)
                {
                    case 0:
                        Debug.Log("Head Position: " + (x + 1) + "-" + y);
                        HeadNotVisible = (x + 1 >= 4);
                        break;
                    case 90:
                        Debug.Log("Head Position: " + x + "-" + (y - 1));
                        HeadNotVisible = (y - 1 < 0);
                        break;
                    case 180:
                        Debug.Log("Head Position: " + (x - 1) + "-" + y);
                        HeadNotVisible = (x - 1 < 0);
                        break;
                    case 270:
                        Debug.Log("Head Position: " + x + "-" + (y + 1));
                        HeadNotVisible = (y + 1 > 6);
                        break;
                }

                //Debug.Log(HeadNotVisible);

                /*
                 * Head Orientation:
                 * 0-> aggiungo 1 a x
                 * 90 -> tolgo 1 a y
                 * 180 -> tolgo 1 a x
                 * 270 -> aggiungo 1 a y
                 */
            }
        }

        old_x = x;
        old_y = y;
    }
}
