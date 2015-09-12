using UnityEngine;
using System.Collections;



public class GameController : MonoBehaviour {
	public Gemstone gemstone; 
	//public GameObject[] gemstones; 
	public int RowNumber;
	public int ColNumber;

	private ArrayList gemstonelist;

	private Gemstone currentgemstone;
	private ArrayList matchedgemstonelist;

    public AudioClip Match3Sound;
    public AudioClip SwapSound;
    public AudioClip ErrorSound;

    private AudioSource audiosource;
    private int refreshtype = 0;//刷新类型
    private bool switch_flag = false;//是否在交换标志

	// Use this for initialization
	void Start () {
		gemstonelist = new ArrayList ();
		matchedgemstonelist = new ArrayList ();
        audiosource = GetComponent<AudioSource>();
		for (int rowindex=0; rowindex < RowNumber; rowindex ++) 
		{
			ArrayList tmp = new ArrayList ();

			for(int colindex = 0; colindex < ColNumber ; colindex ++)
			{
				//GameObject gemstone = gemstones [Random.Range (0, gemstones.Length)];
				//Gemstone c = Instantiate (gemstones [Random.Range (0, gemstones.Length)]) as Gemstone;
				Gemstone c = CreateGemstone(rowindex,colindex);
				tmp.Add (c);//宝石放到列中
			}
			gemstonelist .Add (tmp);//每一列再放到行中
		}
		//消除开始时出现的匹配宝石
		if (CheckRow () || CheckCol ()) 
			RemoveMatchedGemstone ();
        //如果没有可以匹配的宝石，刷新一下
        if (false == FindMatchableGemstone())
        {
            Debug.LogError("Start No matchable gemstone!!! time="+Time.time);
            StartCoroutine(RefreshGemstone());
        }
	}

	public Gemstone CreateGemstone(int rowindex,int colindex){
		Gemstone c = Instantiate (gemstone) as Gemstone;
		c.transform.parent = this .transform;
		c.GetComponent <Gemstone>().CreateRandomGemstone();
		c.GetComponent<Gemstone>().UpdatePosition(rowindex ,colindex);
		return c;
	}

	public Gemstone GetGemstone(int row,int col){
		ArrayList tmp = gemstonelist [row] as ArrayList;
		Gemstone c = tmp [col] as Gemstone;
		return c;
	}

	public void SetGemstone(int row, int col, Gemstone c){
		ArrayList tmp = gemstonelist [row] as ArrayList;
		tmp [col] = c;
	}

	IEnumerator ExchangeAndMatcheGemstone(Gemstone c1, Gemstone c2){
        audiosource.PlayOneShot(SwapSound);
		ExchangeGemstone(c1,c2);
		yield return new WaitForSeconds (0.5f);//延迟0.5s，能看出来交换效果

        switch_flag = false;

		if (CheckRow () || CheckCol ()) 
		{
			RemoveMatchedGemstone ();
		}
		else 
		{
            audiosource.PlayOneShot(SwapSound);
			ExchangeGemstone(c2,c1);
		}
	}

	void ExchangeGemstone(Gemstone c1, Gemstone c2){

		SetGemstone (c1.RowIndex, c1.ColIndex , c2);
		SetGemstone (c2.RowIndex, c2.ColIndex , c1);
		
		int tmprowindex;
		tmprowindex = c1.RowIndex;
		c1.RowIndex = c2.RowIndex;
		c2.RowIndex = tmprowindex;
		
		int tmpcolindex;
		tmpcolindex = c1.ColIndex;
		c1.ColIndex = c2.ColIndex;
		c2.ColIndex = tmpcolindex;
		
        c1.TweenToPosition(c1.RowIndex, c1.ColIndex);//c1.UpdatePosition (c1.RowIndex, c1.ColIndex);
        c2.TweenToPosition(c2.RowIndex, c2.ColIndex);//c2.UpdatePosition (c2.RowIndex, c2.ColIndex);
	}

    void ExchangeGemstoneNoEffect(Gemstone c1, Gemstone c2){
        
        SetGemstone (c1.RowIndex, c1.ColIndex , c2);
        SetGemstone (c2.RowIndex, c2.ColIndex , c1);
        
        int tmprowindex;
        tmprowindex = c1.RowIndex;
        c1.RowIndex = c2.RowIndex;
        c2.RowIndex = tmprowindex;
        
        int tmpcolindex;
        tmpcolindex = c1.ColIndex;
        c1.ColIndex = c2.ColIndex;
        c2.ColIndex = tmpcolindex;
        
        c1.UpdatePosition (c1.RowIndex, c1.ColIndex);
        c2.UpdatePosition (c2.RowIndex, c2.ColIndex);
    }

	void AddMatchedGemstoneToList(Gemstone c){
		if (null == matchedgemstonelist)
			matchedgemstonelist = new ArrayList ();

		int index = matchedgemstonelist .IndexOf (c);//检测该宝石是否在数组中
		if (-1 == index)//不在数组中
			matchedgemstonelist.Add (c);
	}

    void RemoveMatchedGemstone(){
		for (int i=0; i<matchedgemstonelist.Count; i++) 
		{
			Gemstone c = matchedgemstonelist[i] as Gemstone;
            audiosource.PlayOneShot(Match3Sound);
			RemoveGemstone(c);
		}
		matchedgemstonelist = new ArrayList ();
		StartCoroutine (WaitForMatcheAndCheck ());

        if (false == FindMatchableGemstone())
        {
            Debug.LogError("RemoveMatchedGemstone No matchable gemstone!!! time="+Time.time);
            StartCoroutine(RefreshGemstone());
        }
    }
    
	IEnumerator WaitForMatcheAndCheck(){
		yield return new WaitForSeconds (0.5f);
		if (CheckRow () || CheckCol ())
			RemoveMatchedGemstone ();
    }
    
	void RemoveGemstone(Gemstone c){
		Debug.Log ("RemoveGemstone");
		c.ClearGemstone ();
        //audiosource.PlayOneShot(Match3Sound);
		for (int i=c.RowIndex + 1; i<RowNumber; i++) 
		{
			Gemstone tmpgemstone = GetGemstone(i,c.ColIndex);
			tmpgemstone.RowIndex --;
			SetGemstone(tmpgemstone.RowIndex,tmpgemstone.ColIndex,tmpgemstone);
            tmpgemstone.TweenToPosition(tmpgemstone.RowIndex,tmpgemstone.ColIndex);//tmpgemstone.UpdatePosition(tmpgemstone.RowIndex,tmpgemstone.ColIndex);
		}

		Gemstone newgemstone = CreateGemstone (RowNumber, c.ColIndex);
		newgemstone.RowIndex --;
		SetGemstone(newgemstone.RowIndex,newgemstone.ColIndex,newgemstone);
        newgemstone.TweenToPosition(newgemstone.RowIndex, newgemstone.ColIndex);//newgemstone.UpdatePosition(newgemstone.RowIndex,newgemstone.ColIndex);
	}

    IEnumerator RefreshGemstone()
    {
        //return;
        Debug.LogError("RefreshGemstone 1 time=" + Time.time);

        yield return new WaitForSeconds(0.5f);

        int min = Mathf.Min(RowNumber, ColNumber);
        Gemstone c1 = null;
        Gemstone c2 = null;
        while(true)
        {
            Debug.LogError("RefreshGemstone 3 refreshtype=" + refreshtype + " time="+Time.time);
            if(0 == refreshtype || 5 == refreshtype)
            {
                yield return new WaitForSeconds(1.0f);
                for (int row =0; row < min-1; row++)
                {
                    for(int col=row+1; col < min; col++)
                    {
                        {
                            c1 = GetGemstone(row,col);
                            c2 = GetGemstone(col,row);
                            ExchangeGemstone(c1,c2);
                            c1 = null;
                            c2 = null;
                        }
                    }
                }
            }
            else if(1 == refreshtype || 6 == refreshtype)
            {
                yield return new WaitForSeconds(1.0f);
                for (int row =0; row < RowNumber/2; row++)
                {
                    for(int col=0; col < ColNumber; col++)
                    {
                        {
                            c1 = GetGemstone(row,col);
                            c2 = GetGemstone(RowNumber-1-row,col);
                            ExchangeGemstone(c1,c2);
                            c1 = null;
                            c2 = null;
                        }
                    }
                }
            }
            else if(2 == refreshtype || 7 == refreshtype)
            {
                yield return new WaitForSeconds(1.0f);
                for (int col =0; col < ColNumber/2; col++)
                {
                    for(int row=0; row < RowNumber; row++)
                    {
                        {
                            c1 = GetGemstone(row,col);
                            c2 = GetGemstone(row,ColNumber-1-col);
                            ExchangeGemstone(c1,c2);
                            c1 = null;
                            c2 = null;
                        }
                    }
                }
            }
            else if(3 == refreshtype || 8 == refreshtype)
            {
                yield return new WaitForSeconds(1.0f);
                for (int row =0; row < min-1; row++)
                {
                    for(int col=0; (col < min-1) && (row+col<min - 1); col++)
                    {
                        {
                            c1 = GetGemstone(row,col);
                            c2 = GetGemstone(min-1-col,min-1-row);
                            ExchangeGemstone(c1,c2);
                            c1 = null;
                            c2 = null;
                        }
                    }
                }
            }
            else
            {
                int clear_gemstone_type = GetGemstoneType(Random.Range(0,RowNumber-1),Random.Range(0,ColNumber-1));
                for(int row =0; row<RowNumber-1; row++)
                {
                    for(int col =0; col<ColNumber-1; col++)
                    {
                        c1 = null;
                        c1 = GetGemstone(row,col);
                        if(clear_gemstone_type == c1.GemstoneType)
                        {
                            RemoveGemstone(c1);
                        }
                    }
                }
                Debug.LogError("No matchable gemstone!!!! 4 refreshtype= " + refreshtype+"time"+Time.time + " clear_gemstone_type= "+clear_gemstone_type);
            }

            //检测并消除三连的宝石
            if (CheckRow () || CheckCol ())
                RemoveMatchedGemstone ();
            
            refreshtype++;

            if(refreshtype >9)
                refreshtype = 0;

            if(true == FindMatchableGemstone())
            {
                Debug.LogError("RefreshGemstone FindMatchableGemstone time=" + Time.time);
                refreshtype =0;
                break;
            }
        }
    }
    /// <summary>
    /// Checks the row.
    /// </summary>
    /// <returns><c>true</c>, if row was checked, <c>false</c> otherwise.</returns>
	bool CheckRow(){
		bool matched = false;
        int current_gemtype = -1;

		for(int row = 0; row < RowNumber; row ++)
		{
			for( int col = 0; col < ColNumber -2; col++)
			{
                current_gemtype = GetGemstoneType(row,col);

                if((current_gemtype == GetGemstone(row,col+1).GemstoneType)
                   && (current_gemtype == GetGemstone(row,col+2).GemstoneType))
				{
					Debug.Log("Finded same gemstong in row");
					matched = true;
					AddMatchedGemstoneToList(GetGemstone(row,col));
					AddMatchedGemstoneToList(GetGemstone(row,col+1));
					AddMatchedGemstoneToList(GetGemstone(row,col+2));

                    CheckAndAddVerticalMatchedGemstone(row,col,current_gemtype);
				}
			}
		}
		return matched;
	}

	bool CheckCol(){
		bool matched = false;
        int current_gemtype = -1;
		for(int col = 0; col < ColNumber; col ++)
		{
			for( int row = 0; row < RowNumber -2; row++)
			{
                current_gemtype = GetGemstoneType(row,col);

                if((current_gemtype == GetGemstone(row+1,col).GemstoneType)
                   && (current_gemtype == GetGemstone(row+2,col).GemstoneType))
				{
					Debug.Log("Finded same gemstong in col");
					matched = true;
					AddMatchedGemstoneToList(GetGemstone(row,col));
					AddMatchedGemstoneToList(GetGemstone(row+1,col));
					AddMatchedGemstoneToList(GetGemstone(row+2,col));

                    CheckAndAddHorizontalMatchedGemstone(row,col,current_gemtype);
				}
			}
		}
		return matched;
	}

	public void SelectGemstone(Gemstone c){
		//Destroy (c.gameObject);
        if (true == switch_flag)
            return;

		if (currentgemstone == null) 
		{
			currentgemstone = c;
            currentgemstone .IsGemstoneSelected(true);// = true;
			//currentgemstone.ShakeToPosition();
		}
		else
		{
			if (1 == (Mathf.Abs (currentgemstone.RowIndex - c.RowIndex) + Mathf.Abs (currentgemstone.ColIndex - c.ColIndex)))//相邻的
			{
                switch_flag = true;
		        StartCoroutine(ExchangeAndMatcheGemstone (currentgemstone, c));
			}
            else
            {
                audiosource.PlayOneShot(ErrorSound);
            }
            currentgemstone .IsGemstoneSelected(false);// = false;
			currentgemstone = null ;
		}
	}

    int GetGemstoneType(int row, int col)
    {
        if (row < 0 || row > RowNumber - 1 || col < 0 || col > ColNumber - 1)
            return -1;
        else
            return GetGemstone(row, col).GemstoneType;
    }

    /// <summary>
    /// Checks the and add upside and downside matched gemstone.
    /// 2    2    2   1      1      1
    /// o    o    o   
    /// o    o    o   o      o      o
    /// ooo ooo ooo   ooo   ooo   ooo
    /// o    o    o   o      o      o
    /// o    o    o   
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    /// <param name="type">Type.</param>
    /// 2015-9-12 15:18:14 create function
    void CheckAndAddVerticalMatchedGemstone(int row, int col, int type)
    {
        for(int tmp_col=col;col<=tmp_col+2;col++)
        {
            if (type == GetGemstoneType(row - 1, col) && type == GetGemstoneType(row - 2, col))
            {
                AddMatchedGemstoneToList(GetGemstone(row - 1, col));
                AddMatchedGemstoneToList(GetGemstone(row - 2, col));
            }

            if (type == GetGemstoneType(row + 1, col) && type == GetGemstoneType(row + 2, col))
            {
                AddMatchedGemstoneToList(GetGemstone(row + 1, col));
                AddMatchedGemstoneToList(GetGemstone(row + 2, col));
            }

            if (type == GetGemstoneType(row - 1, col) && type == GetGemstoneType(row + 1, col))
            {
                AddMatchedGemstoneToList(GetGemstone(row + 1, col));
                AddMatchedGemstoneToList(GetGemstone(row + 2, col));
            }
        }
    }
    /// <summary>
    /// Checks the and add leftside and rightside matched gemstone.
    ///  2     2     2     1     1     1
    ///ooooo   o     o    ooo    o     o
    ///  o   ooooo   o     o    ooo    o  
    ///  o     o   ooooo   o     o    ooo
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    /// <param name="type">Type.</param>
    /// 2015-9-12 15:53:04 create function
    void CheckAndAddHorizontalMatchedGemstone(int row, int col, int type)
    {
        for(int tmp_col=col;col<=tmp_col+2;col++)
        {
            if (type == GetGemstoneType(row, col - 1) && type == GetGemstoneType(row, col- 2))
            {
                AddMatchedGemstoneToList(GetGemstone(row, col - 1));
                AddMatchedGemstoneToList(GetGemstone(row, col - 2));
            }

            if (type == GetGemstoneType(row, col+ 1) && type == GetGemstoneType(row, col + 2))
            {
                AddMatchedGemstoneToList(GetGemstone(row, col + 1));
                AddMatchedGemstoneToList(GetGemstone(row, col + 2));
            }

            if (type == GetGemstoneType(row, col - 1) && type == GetGemstoneType(row, col + 1))
            {
                AddMatchedGemstoneToList(GetGemstone(row, col + 1));
                AddMatchedGemstoneToList(GetGemstone(row, col + 2));
            }
        }
    }
    /// <summary>
    /// Checks the left matchable gemstone.
    /// </summary>
    /// <returns><c>true</c>, if left matchable gemstone was checked, <c>false</c> otherwise.</returns>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    /// <param name="type">Type.</param>
    bool CheckLeftMatchableGemstone(int row, int col, int type)
    {
        if (type == GetGemstoneType(row + 1, col - 1) ||
            type == GetGemstoneType(row - 1, col - 1) ||
            type == GetGemstoneType(row, col - 2)
           )
            return true;
        else
            return false;
    }

    bool CheckRightMatchableGemstone(int row, int col, int type)
    {
        if (type == GetGemstoneType(row + 1, col + 1) ||
            type == GetGemstoneType(row - 1, col + 1) ||
            type == GetGemstoneType(row, col + 2)
            )
            return true;
        else
            return false;
    }

    bool CheckUpMatchableGemstone(int row, int col, int type)
    {
        if (type == GetGemstoneType(row - 1, col - 1) ||
            type == GetGemstoneType(row - 2, col) ||
            type == GetGemstoneType(row - 1, col + 1)
            )
            return true;
        else
            return false;
    }

    bool CheckDownMatchableGemstone(int row, int col, int type)
    {
        if (type == GetGemstoneType(row + 1, col - 1) ||
            type == GetGemstoneType(row + 2, col) ||
            type == GetGemstoneType(row + 1, col + 1)
            )
            return true;
        else
            return false;
    }

    bool FindMatchableGemstone()
    {
        bool matchable = false;

        int current_gemtype = -1;
        int right1_gemtype = -1;
        int right2_gemtype = -1;
        int down1_gemtype = -1;
        int down2_gemtype = -1;
        Debug.LogError("FindMatchableGemstone time=" + Time.time);
        for(int row = 0; row < RowNumber; row ++)
        {
            for( int col = 0; col < ColNumber -1; col++)
            {
                current_gemtype = GetGemstoneType(row,col);
                right1_gemtype = GetGemstoneType(row,col+1);
                right2_gemtype = GetGemstoneType(row,col+2);
                down1_gemtype = GetGemstoneType(row+1,col);
                down2_gemtype = GetGemstoneType(row+2,col);

                if(current_gemtype == right1_gemtype)
                {
                    if(CheckLeftMatchableGemstone(row,col,current_gemtype) || CheckRightMatchableGemstone(row,col+1,current_gemtype))
                    {
                        Debug.Log("Finded matchable gemstone in row=" + (row+1));
                        matchable = true;
                        return matchable;
                    }
                }
                else if(current_gemtype == right2_gemtype)
                {
                    if(current_gemtype == GetGemstoneType(row-1,col+1) || current_gemtype == GetGemstoneType(row+1,col+1))
                    {
                        Debug.Log("Finded matchable gemstone in row=" + (row+1));
                        matchable = true;
                        return matchable;
                    }
                }
                else if(current_gemtype == down1_gemtype)
                {
                    if(CheckUpMatchableGemstone(row,col,current_gemtype) || CheckDownMatchableGemstone(row+1,col,current_gemtype))
                    {
                        Debug.Log("finede matchable gemstone in col=" + (col+1));
                        matchable = true;
                        return matchable;
                    }
                }
                else if(current_gemtype == down2_gemtype)
                {
                    if(current_gemtype == GetGemstoneType(row + 1,col - 1) || current_gemtype == GetGemstoneType(row + 1,col + 1))
                    {
                        Debug.Log("finede matchable gemstone in col=" + (col+1));
                        matchable = true;
                        return matchable;
                    }
                }
            }
        }
        Debug.LogError("FindMatchableGemstone no matchable gemstone !!!!!!="+ matchable + " time=" + Time.time);
        return matchable;
    }

}
