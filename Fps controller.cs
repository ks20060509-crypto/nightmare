using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class fpscontroller : MonoBehaviour
{
    //移動用の変数を作成
    float x, z;
    float nextFireTime;
    public float fireRate = 0.2f; // 100ms between shots

    //スピード調整用の変数を作成
    public float speed = 0.2f;

    //ゲームオブジェクト・回転・マウス感度調整用変数の宣言
    public GameObject cam;
    Quaternion cameraRot,characterRot;

    float Xsensitivity = 3f, Ysensitivity = 3f;

    //変数の宣言
    bool cursorLock;

    //変数の宣言（角度の制現用）
    float minX = -90f, maxX = 90f;

    //変数の宣言（アニメーション用）
    public Animator animator;

    //所持弾薬、最高所持弾薬、マガジン内の弾数、マガジン内の最大数(250発で１５回撃てて、１０００で５０発文撃てる）

    int ammunition = 50, maxAmmunition = 50, ammoClip = 10, maxAmmoClip = 10;

    //体力、弾薬の変数の宣言
    int playerHP = 100, maxPlayerHP = 100;
    public Slider hpber;
    public Text ammocText;

    public GameObject mainCamera, subCamera;

    public AudioSource playerFootStep;
    public AudioClip walkFootStepSe, runFootStepSe;

    public AudioSource weapon;
    public AudioClip reloadingSe, fireSe, triggerSe;

    //入力に合わせてプレイヤーの位置を変更していく


    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        GameState.canShoot = true;

        hpber.value = playerHP;
        ammocText.text = ammoClip + "/" + ammunition;




    }


    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensitivity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        GameState.canShoot = true;

        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock();  



        if (Input.GetMouseButton(0) && Time.time > nextFireTime)
        {
            Weapon.instance.FireSe();

           



            if (ammoClip > 0)
                {
                    animator.SetTrigger("Fire");
                GameState.canShoot = false;
       
                ammoClip --;
                ammocText.text = ammoClip + "/" + ammunition;

                Debug.Log("ammoClip = " + ammoClip);

                nextFireTime = Time.time + fireRate;

                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                { Debug.Log(hit.collider.name);
                    ZomieController zombie = hit.collider.GetComponentInParent<ZomieController>();

                    if (zombie != null)
                    {
                        if (hit.collider.CompareTag("HEAD"))
                        {
                            Debug.Log("頭に命中");
                            zombie.zombieTakeDamage(2);

                        }
                        else if (hit.collider.CompareTag("BODY"))
                        {
                            Debug.Log("胴体に命中！");
                            zombie.zombieTakeDamage(1);
                        }
                    }

                }

               

            }
                else 
                {
                //Debug.Log("弾切れです。");

                Weapon.instance.TriggerSe();
                }   
            
            
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Weapon.instance.ReloadingSe();
            int amountNeed = maxAmmoClip - ammoClip;
            int ammoAvailable = amountNeed < ammunition ? amountNeed : ammunition;
            if(amountNeed !=0 && ammunition !=0)
            {
                animator.SetTrigger("Reload");

                ammunition -= ammoAvailable;
                ammoClip += ammoAvailable;
                ammocText.text = ammoClip + "/" + ammunition;

            }
           
        }

        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            
            if (!animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", true);

                PlayerWalkStep(walkFootStepSe);
            }

            else if (animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", false);

                StopFootStep();
            }

            if (z > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                if (!animator.GetBool("Run"))
                {
                    animator.SetBool("Run", true);
                    speed = 0.25f;

                    PlayerRunFootStep(runFootStepSe);
                }

                else if (animator.GetBool("Run"))
                {
                    animator.SetBool("Run", false);
                    speed  = 0.1f;

                    StopFootStep();
                }

                

            }
        }
        if (Input.GetMouseButton(1))
        {
            subCamera.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = false;
        }

        else if (subCamera.activeSelf)
        {
            subCamera.SetActive(false);
            mainCamera.GetComponent<Camera>().enabled = true;

        }

    }
    private void FixedUpdate() //0.02秒ごとに呼ばれる
    {
        x = 0;
        z = 0;
        
        

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x,0,z);

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 move = forward * z + right * x;

        if (move != Vector3.zero)
        {
            transform.position += move.normalized * speed;
        }





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

    public void PlayerWalkStep(AudioClip clip)
    {
        playerFootStep.loop = true;
        playerFootStep.pitch = 1f;
        playerFootStep.clip = clip;
        playerFootStep.Play();

    }

    public void PlayerRunFootStep(AudioClip clip)
    {
        playerFootStep.loop = true;
        playerFootStep.pitch = 1.3f;
        playerFootStep.clip = clip;
        playerFootStep.Play();

    }

    public void StopFootStep()
    {
        playerFootStep.Stop();
        playerFootStep.loop = false;
        playerFootStep.pitch = 1f;

    }

    public void TakeDamage(int damage)
    {
        playerHP -= damage;
        hpber.value = playerHP;
        
    }



}
