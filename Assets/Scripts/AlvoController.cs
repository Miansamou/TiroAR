using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlvoController : MonoBehaviour
{
    public float tempoAlvoCriado = 0;
    private bool movimentar = false;
    private bool rotacionar = false;
    
    // Start is called before the first frame update
    void Start()
    {
        int porcentagem = Random.Range(1, 10);
        if (porcentagem == 1)
        {
            movimentar = true;
        }
        if (porcentagem == 2)
        {
            rotacionar = true;
        }
        if (porcentagem == 3)
        {
            movimentar = true;
            rotacionar = true;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        Timer();
        if (rotacionar) transform.Rotate(Vector3.left);
        if (movimentar)
        {
            if ((int)tempoAlvoCriado % 2 == 0)
            {
                transform.Translate(Vector3.forward * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.back * Time.deltaTime);
            }
        }

        ApagarAlvoRuim();
    }

    private void ApagarAlvoRuim()
    {
        if (tempoAlvoCriado >= 9 && gameObject.tag.Equals("AlvoRuim"))
        {
            Destroy(this);
        }
    }
    
    private void Timer()
    {
        tempoAlvoCriado += Time.deltaTime;
        
        if ( tempoAlvoCriado > 9 )
        {
            tempoAlvoCriado = 9;
        }
    }
}
