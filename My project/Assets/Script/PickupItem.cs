using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Potion, Knife }
    public ItemType itemType;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Myscript1 player = collision.GetComponent<Myscript1>();
            if (player != null)
            {
                bool pickedUp = false;

                if (itemType == ItemType.Potion)
                {
                    if (player.Potion < player.maxPotion)  // ✅ ยังไม่เต็ม
                    {
                        player.AddPotion(amount);
                        pickedUp = true;
                    }
                }
                else if (itemType == ItemType.Knife)
                {
                    if (player.Knife < player.maxKnife)   // ✅ ยังไม่เต็ม
                    {
                        player.AddKnife(amount);
                        pickedUp = true;
                    }
                }

                if (pickedUp)
                {
                    // ✅ ลบไอเทมออกเฉพาะตอนที่เก็บสำเร็จ
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("ของเต็มแล้ว! เก็บไม่ได้");
                }
            }
        }
    }
}