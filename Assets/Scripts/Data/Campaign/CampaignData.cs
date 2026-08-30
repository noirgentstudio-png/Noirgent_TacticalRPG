[System.Serializable]
public class CampaignData
{
    public string campaignId;
    public int seed;
    public CampaignTime time;
    public string commanderId;
    public string companyId;

    public CampaignData()
    {
        campaignId = System.Guid.NewGuid().ToString();
        seed = UnityEngine.Random.Range(0, 999999);
        time = new CampaignTime();
        commanderId = "";
        companyId = "";
    }
}