using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    private ARPlane _plane;
    private GameObject _cursor;
    public GameObject alvoPrefab;
    public GameObject alvoPrefabRuim;
    public int alvosCriar = 10;
    public int pontuacao;
    public bool acabou;
    public Text msg;
    public Text tempotxt;
    public Text pontostxt;
    public AudioClip destroyObjGood;
    public AudioClip destroyObjBad;
    public AudioSource Source;
    public Image alvoImg;
    private Color _changeColor;
    public float contador = 30;
    private bool _comecouJogo;

    // Start is called before the first frame update
    void Start()
    {
        _cursor = transform.Find("Cursor").gameObject;
        msg.text = "Mapeie o chão e toque na tela para começar";
        alvoImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!acabou)
        {
            TratarPlanos();
            TratarMira();
        }
        else
        {
            TratarFimJogo();
            msg.text = "Fim de Jogo\n\nSua pontuação é de " + pontuacao;
            alvoImg.gameObject.SetActive(false);
        }

        if (contador % 2 == 0)
        {
            CriarAlvoRuim();
        }

        TratarCorAlvo();
        Timer();
    }

    private void Timer()
    {
        if (_comecouJogo)
        {
            contador -= Time.deltaTime;
            if ( contador <= 0 )
            {
                contador = 0;
                acabou = true;
            }

            tempotxt.text = ((int)contador).ToString();
        }
    }
    
    private void TratarCorAlvo()
    {
        RaycastHit hitInfo;
        var camera = Camera.main.transform;
        _changeColor.b = 1;
        _changeColor.g = 1;
        _changeColor.r = 1;
        alvoImg.color = _changeColor;
        if (Physics.Raycast(camera.position, camera.forward, out hitInfo, 100000))
        {
            if (hitInfo.transform.CompareTag("Alvo") || hitInfo.transform.CompareTag("AlvoRuim"))
            {
                _changeColor.b = 0;
                _changeColor.g = 0;
                alvoImg.color = _changeColor;
            }
        }
    }
    
    private void TratarMira()
    {
        RaycastHit hitInfo;
        var camera = Camera.main.transform;
        if (Physics.Raycast(camera.position, camera.forward, out hitInfo, 100000))
        {
            if (hitInfo.transform.CompareTag("Alvo") && Input.touchCount == 1 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Destroy(hitInfo.transform.gameObject);
                Source.PlayOneShot(destroyObjGood);
                TratarAcerto(10, hitInfo.transform.gameObject.GetComponent<AlvoController>());
            }
            else if (hitInfo.transform.CompareTag("AlvoRuim") && Input.touchCount == 1 &&
                     Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Destroy(hitInfo.transform.gameObject);
                Source.PlayOneShot(destroyObjBad);
                TratarAcerto(-10, hitInfo.transform.gameObject.GetComponent<AlvoController>());
            }
        }
    }
    
    private void TratarAcerto(int valor, AlvoController alvo)
    {
        pontuacao += valor - (int)alvo.tempoAlvoCriado;
        pontostxt.text = "Pontos: " + pontuacao;
        alvosCriar--;
        if (alvosCriar > 0)
        {
            CriarAlvo();
        }
        else
        {
            acabou = true;
        }
    }

    private void TratarPlanos()
    {
        if (_plane != null)
        {
            return;
        }

        //Cria um vetor no apontando para o meio da tela
        var screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        //Cria um array de hits do traçado de raios AR, para armazenar colisões com planos
        var raycastHits = new List<ARRaycastHit>();
        //Solicita o traçado de raios partindo do meio da tela e colidindo com planos
        raycastManager.Raycast(screenPosition, raycastHits,
            UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);
        //Se colidiu com algo, atualiza a posição do objeto para o local
        if (raycastHits.Count > 0)
        {
            transform.position = raycastHits[0].pose.position;
            transform.rotation = raycastHits[0].pose.rotation;
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ComecarJogo(raycastHits);
            }
        }
    }
    
    private void ComecarJogo(List<ARRaycastHit> raycastHits)
    {
        _comecouJogo = true;
        msg.text = "";
        alvoImg.gameObject.SetActive(true);
        var planeId = raycastHits[0].trackableId;
        _plane = planeManager.GetPlane(planeId);
        CriarAlvo();
        _cursor.SetActive(false);
        planeManager.enabled = false;
    }
    
    public void CriarAlvo()
    {
        Vector3 pos = ObterPontoAleatorio();
        int numeroAlvos = this.NumeroAlvos();
        for (int i = 0; i <= numeroAlvos; i++)
        {
            GameObject target = GameObject.Instantiate(alvoPrefab, pos, Quaternion.identity);
            target.transform.LookAt(Camera.main.transform);
        }
    }
    
    public void CriarAlvoRuim()
    {
        Vector3 pos = ObterPontoAleatorio();
        GameObject target = GameObject.Instantiate(alvoPrefabRuim, pos, Quaternion.identity);
        target.transform.LookAt(Camera.main.transform);
    }

    private int NumeroAlvos()
    {
        int porcentagem = Random.Range(1, 30);
        int instancia = 1;
        if (porcentagem <= 3)
        {
            instancia = 2;
            if (porcentagem == 1)
            {
                instancia = 3;
            }
        }
        return instancia;
    }
    
    private Vector3 ObterPontoAleatorio()
    {
        var vetorAleatorio = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.1f, 1),
            Random.Range(-0.5f, 0.5f));
        Vector3 posicaoAleatoria = _plane.transform.TransformPoint(vetorAleatorio);
        return posicaoAleatoria;
    }
    
    private void TratarFimJogo()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
