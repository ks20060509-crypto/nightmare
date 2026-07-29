using UnityEngine;

public class fpscontroller : MonoBehaviour
{
    //移動用の変数を作成
    float x, z;

    //スピード調整用の変数を作成
    float speed = 0.1f;

    //ゲームオブジェクト・回転・マウス感度調整用変数の宣言
    public GameObject cam;
    Quaternion cameraRot,characterRot;

    float Xsensitivity = 3f, Ysensitivity = 3f;

    //変数の宣言
    bool cursorLock;

    //変数の宣言（角度の制現用）
    float minX = -90f, maxX = 90f;

    //入力に合わせてプレイヤーの位置を変更していく


    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;


    }

    
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensitivity;   

        cameraRot *= Quaternion.Euler(-yRot ,0,0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock(); 
    }

    private void FixedUpdate() //0.02秒ごとに呼ばれる
    {
        x = 0;
        z = 0;
        
        

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x,0,z);

        transform.position += transform.forward * z + cam.transform.right * x;  

    }

    public void UpdateCursorLock()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;

        }

        else if (Input.GetMouseButtonDown(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public Quaternion ClampRotation(Quaternion q)
    {
        //q=x,y,z,w (x,y,zはベクトル（量と向きを表す）、wはスカラー（量のみ）を表す)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q. x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

}
