using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlossarioSystem : MonoBehaviour
{
    public GlossarioData glossarioData;
    public TextMeshProUGUI nomeText;
    public TextMeshProUGUI descricaoText;
    public Image imagemPersonagem;

    private int pagina = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            nextPage();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            prevPage();
        }

        // Garante que a página está dentro dos limites do array
        pagina = Mathf.Clamp(pagina, 0, glossarioData.glossaryScript.Count - 1);

        // Atualiza o texto e a imagem com as informações do personagem na página atual
        nomeText.text = glossarioData.glossaryScript[pagina].name;
        descricaoText.text = glossarioData.glossaryScript[pagina].description;
        imagemPersonagem.sprite = glossarioData.glossaryScript[pagina].image;
    }

    public void nextPage()
    {
        pagina = (pagina + 1) % glossarioData.glossaryScript.Count;
    }

    public void prevPage()
    {
        pagina = (pagina - 1 + glossarioData.glossaryScript.Count) % glossarioData.glossaryScript.Count;
    }

}
