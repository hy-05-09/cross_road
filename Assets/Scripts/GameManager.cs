using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int Width = 8;
    public int Height = 5;
    public GameObject Platform;
    public GameObject Character;
    public Transform Platform_Parents;
    private List<GameObject> Platform_List = new List<GameObject>();
    private List<int> Platform_Check_List = new List<int>();
    public GameObject Deadline;
    public float Deadline_Speed = 1.0f; //다가오는 벽의 스피드
    public float Deadline_Speed_Max = 2.0f;
    public float Deadline_Speed_Accel = 0.1f; //가속도도
    public int Score; 
    public Text Score_Text;



    // Start is called before the first frame update
    void Start()
    {
        Deadline.GetComponent<MeshRenderer>().material.color = Color.black;
        Data_Load();
        Init();
    }

    void Data_Load(){
        for(int i=0;i<Width*Height;i++)
        {
            GameObject t_Obj = Instantiate(Platform, Vector3.zero, Quaternion.identity);
            t_Obj.transform.parent=Platform_Parents;
            Platform_List.Add(t_Obj);
            Platform_Check_List.Add(0);
        }
        Platform.SetActive(false);
    }

    private bool Game_Start = false;

    public void Init()
    {
        for(int h=0;h<Height;h++)
        {
            for(int w=0;w<Width;w++)
            {
                Platform_List[Width*h+w].transform.position = new Vector3(-(Width -1)/2f+w, -0.5f, h);
                Set_Platform(Width*h+w, 0);
            }
        }
        Character.transform.position = new Vector3(0f, 0.5f, 0f);
        Deadline.transform.position = new Vector3(0f, 0.5f, -3f); //초기 위치
        Deadline.transform.localScale = new Vector3(Width, 1f, 1f); //부모 상대적 크기

        Score=0;
        Score_Text.text = Score.ToString();


        Game_Start = false;
    }



    // Update is called once per frame
    void Update()
    {
        if(Game_Start){
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Move(0);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                Move(1);
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                Move(2);
            }
            Deadline.transform.position += Vector3.forward * Deadline_Speed * Time.deltaTime;
            //Vector3.forward는 (0,0,1)이기 때문문
            if (Deadline_Speed < Deadline_Speed_Max){
                Deadline_Speed += Deadline_Speed_Accel;
            } 
            // else  if (Deadline_Speed < 70){
            //     Deadline_Speed_Accel = 3f;
            //     Deadline_Speed += Deadline_Speed_Accel;
            // }else  if (Deadline_Speed < 100){
            //     Deadline_Speed_Accel = 4f;
            //     Deadline_Speed += Deadline_Speed_Accel;
            // } else  if (Deadline_Speed < Deadline_Speed_Max){
            //     Deadline_Speed_Accel = 7f;
            //     Deadline_Speed += Deadline_Speed_Accel;
            // }
        }else{
            if(Input.GetKeyDown(KeyCode.Space)){
                Init();
                Game_Start = true;
            }
        }
        
    }

     public void Move(int direction)
    {
        bool next_Platform = false; ///캐릭터의 전진 여부부
        switch(direction)
        {
            case 0:
                if(Restrict(Vector3.left)){
                    Character.transform.position += Vector3.left; //Vector3(-1,0,0)
                }
                break;
            case 1:
                if(Restrict(Vector3.right)){
                    Character.transform.position += Vector3.right; //Vector3(1,0,0)
                }
                break;
            case 2:
                if(Restrict(Vector3.forward)){
                    Character.transform.position += Vector3.forward; //Vector3(0,0,1)
                    next_Platform = true;
                }
                break;
        }

        Check_Platform((int)(Width*(Character.transform.position.z % Height) + Character.transform.position.x + Width /2));

        if(next_Platform)
        {
            Next_Platform((int)Character.transform.position.z);
        }
    }

    void Check_Platform(int idx)
    {
        if(Platform_Check_List[idx] == 1)
        {
            Result();
        }
    }

    void Next_Platform(int character_z)
    {
        for(int i=0;i<Width;i++)
        {
            Platform_List[((character_z -4) % Height) * Width + i].transform.position = new Vector3(-Width / 2 +i, -0.5f,
            (character_z -4) + Height);
            Set_Platform((((character_z -4) % Height) * Width + i), UnityEngine.Random.Range(0,8));
        }

        Score++;
        Score_Text.text = Score.ToString();
    }

    bool Restrict(Vector3 direction)
    {
        Vector3 move_Pos = Character.transform.position + direction;
        if(move_Pos.x > Width/2 || move_Pos.x < -Width/2){
            return false;
        }else{
            return true;
        }
    }

    void Set_Platform(int idx, int randomNum)
    {
        switch(randomNum)
        {
            case 1: //함정일 경우
                Platform_List[idx].GetComponent<MeshRenderer>().material.color = Color.red;
                Platform_Check_List[idx] = 1;
                break;
            default :
                Platform_List[idx].GetComponent<MeshRenderer>().material.color = Color.green;
                Platform_Check_List[idx] = 0;
                break;
        }
        
    }

    public void Result(){
        Debug.Log("Game over");
        Game_Start = false;
    }


}

   