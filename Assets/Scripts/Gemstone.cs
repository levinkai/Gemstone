using UnityEngine;
using System.Collections;

public class Gemstone : MonoBehaviour {
	public GameObject[] GemstoneTypeArray;//gemstonesbgs
	public int GemstoneType;
	private GameObject GemstoneBg;//gemstonebg

	public float x_offset;
	public float y_offset;
	public float space;

	public int RowIndex;
	public int ColIndex;

	private GameController gamecontrollerscript;
	private  SpriteRenderer spriterenderer;
    private Transform gem_transform;

    public void IsGemstoneSelected(bool flag)
    {
        if(flag)
        {
            //spriterenderer.color = Color.red;
            gem_transform.localScale += new Vector3(0.3f,0.3f,0);
        }   
        else
        {
            //spriterenderer.color = Color.white;
            gem_transform.localScale -= new Vector3(0.3f,0.3f,0);
        }
    }
/*
	public bool IsGemstoneSelected{
		set{
			if(value ){
				//spriterenderer.color = Color.red;
                this.transform.localScale.x =1;
                this.transform.localScale.y =1;
			}else
            {
				//spriterenderer.color = Color.white;
                this.transform.localScale.x =0.7;
                this.transform.localScale.y =0.7;
            }
		}
	}*/

//	// Use this for initialization
//	void Awake () {
//		spriterenderer = gemstone.GetComponent<spriterenderer > ();
//	}

	void Start () {
		GameObject gamecontrollerobject = GameObject.FindWithTag ("GameController");

		if (gamecontrollerobject != null)
			gamecontrollerscript = gamecontrollerobject.GetComponent <GameController> ();
		
		if(gamecontrollerscript == null)
			Debug.Log("Cannot find script 'Gametransformr'!");

		//getransformthis .gameObject;

		//if(gemstone == null)
			//Debug.Log("Cannot find Gameboject 'gemstone'!");

		if(GemstoneBg == null)
			Debug.Log("Cannot find Gameboject 'GemstoneBg'!");

		if (GemstoneBg)
        {
            spriterenderer = GemstoneBg.GetComponent<SpriteRenderer>();
            gem_transform = GemstoneBg.GetComponent<Transform>();
        }

		if(spriterenderer == null)
			Debug.Log("Cannot find Spriterenderer 'spriterenderer'!");

        if (null == transform)
            Debug.Log("Cannot find Transform 'transform'!");
	}

	public void UpdatePosition(int row, int col){

		RowIndex = row;
		ColIndex = col;

		this.transform.position = new Vector3 (col*space + y_offset, row*space + x_offset ,0);
	}

    public void TweenToPosition(int row, int col){
        RowIndex = row;
        ColIndex = col;
        iTween.MoveTo(this.gameObject, iTween.Hash("x",ColIndex*space + y_offset,"y",RowIndex*space + x_offset,"time",0.5f));
    }

    public void ShakeToPosition()
    {
        //iTween.ShakePosition(this.gameObject, iTween.Hash("x",this.transform.position.x,"y",this.transform.position.y,"time",0.5f));
        iTween.ShakeScale(this.gameObject, iTween.Hash("x",this.transform.position.x,"y",this.transform.position.y,"time",0.5f));
    }

	public void CreateRandomGemstone(){
		if (GemstoneBg != null)
			return;

		GemstoneType = Random .Range (0, GemstoneTypeArray.Length);

		GemstoneBg = Instantiate(GemstoneTypeArray[GemstoneType]) as GameObject ;

		if(GemstoneBg == null)
			Debug.Log("GemstoneBg is null");

		GemstoneBg.transform .parent = this .transform;

	}

	public void OnMouseDown(){
		gamecontrollerscript.SelectGemstone (this);
	}

	public void ClearGemstone(){
		if(null != this)
			Destroy (this.gameObject);
		if(null != GemstoneBg)
			Destroy (GemstoneBg.gameObject);
		if(null != gamecontrollerscript)
			gamecontrollerscript = null;
	}
}
