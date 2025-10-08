using UnityEngine;

namespace CrowdControl;

public class CustomGUIMessages : MonoBehaviour
{
    public enum Language
    {
        English = 0,
        French = 1,
        German = 2,
        Italian = 3,
        Spanish = 4,
        Portugal = 5,
        Brazil = 6,
        Netherlands = 7,
        Turkey = 8
    }

    private readonly Dictionary<string, Dictionary<Language, string>> flagMessages = new()
    {
        {
            "ForceUseCash", new()
            {
                { Language.English, "All customers only have cash." },
                { Language.French, "Tous les clients n'ont que de l'argent liquide." },
                { Language.German, "Alle Kunden haben nur Bargeld." },
                { Language.Italian, "Tutti i clienti hanno solo contanti." },
                { Language.Spanish, "Todos los clientes solo tienen efectivo." },
                { Language.Portugal, "Todos os clientes só têm dinheiro." },
                { Language.Brazil, "Todos os clientes só têm dinheiro." },
                { Language.Netherlands, "Alle klanten hebben alleen contant geld." },
                { Language.Turkey, "Tüm müşterilerin sadece nakit parası var." }
            }
        },
        {
            "ForceUseCredit", new()
            {
                { Language.English, "All customers only have card." },
                { Language.French, "Tous les clients n'ont que des cartes." },
                { Language.German, "Alle Kunden haben nur Karten." },
                { Language.Italian, "Tutti i clienti hanno solo carta." },
                { Language.Spanish, "Todos los clientes solo tienen tarjeta de credito." },
                { Language.Portugal, "Todos os clientes só têm cartão." },
                { Language.Brazil, "Todos os clientes só têm cartão." },
                { Language.Netherlands, "Alle klanten hebben alleen een kaart." },
                { Language.Turkey, "Tüm müşterilerin sadece kartı var." }
            }
        },
        {
            "ForceExactChange", new()
            {
                { Language.English, "All customers will pay in exact change." },
                { Language.French, "Tous les clients paieront avec l'appoint exact." },
                { Language.German, "Alle Kunden zahlen mit genauem Wechselgeld." },
                { Language.Italian, "Tutti i clienti pagheranno con il resto esatto." },
                { Language.Spanish, "Todos los clientes pagarán con el cambio exacto." },
                { Language.Portugal, "Todos os clientes pagarão com o troco exato." },
                { Language.Brazil, "Todos os clientes pagarão com o troco exato." },
                { Language.Netherlands, "Alle klanten betalen met precies wisselgeld." },
                { Language.Turkey, "Tüm müşteriler tam para üstüyle ödeyecek." }
            }
        },
        {
            "ForceRequireChange", new()
            {
                { Language.English, "All customers will not pay in exact change." },
                { Language.French, "Tous les clients ne paieront pas avec l'appoint exact." },
                { Language.German, "Alle Kunden werden nicht mit dem genauen Betrag bezahlen." },
                { Language.Italian, "Tutti i clienti non pagheranno con il resto esatto." },
                { Language.Spanish, "Todos los clientes no pagarán con el cambio exacto." },
                { Language.Portugal, "Todos os clientes não pagarão com o troco exato." },
                { Language.Brazil, "Todos os clientes não vão pagar com o troco exato." },
                { Language.Netherlands, "Niet alle klanten zullen met het exacte wisselgeld betalen." },
                { Language.Turkey, "Tüm müşteriler tam para üstü ile ödeme yapmayacak." }
            }
        },
        {
            "AllowMischarge", new()
            {
                { Language.English, "You can currently overcharge card payments." },
                { Language.French, "Vous pouvez actuellement surcharger les paiements par carte." },
                { Language.German, "Sie können derzeit Kartenzahlungen überladen." },
                { Language.Italian, "Attualmente puoi addebitare eccessivamente i pagamenti con carta." },
                { Language.Spanish, "Actualmente puedes sobrecargar los pagos con tarjeta de credito." },
                { Language.Portugal, "Atualmente, você pode sobrecarregar os pagamentos com cartão." },
                { Language.Brazil, "Atualmente, você pode sobrecarregar os pagamentos com cartão." },
                { Language.Netherlands, "U kunt momenteel kaartbetalingen te veel in rekening brengen." },
                { Language.Turkey, "Şu anda kart ödemelerinden fazla ücret alabilirsiniz." }
            }
        }
    };

    private List<string> activeMessages = new();

    void Update()
    {
        UpdateActiveMessages();
    }

    void UpdateActiveMessages()
    {
        if (GameStateManager.CurrentLanguage > 8) GameStateManager.CurrentLanguage = 0;
        Language currentLanguage = (Language)GameStateManager.CurrentLanguage;

        activeMessages.Clear();
        if (GameStateManager.ForceUseCash) activeMessages.Add(flagMessages["ForceUseCash"][currentLanguage]);
        if (GameStateManager.ForceUseCredit) activeMessages.Add(flagMessages["ForceUseCredit"][currentLanguage]);
        if (GameStateManager.ForceExactChange) activeMessages.Add(flagMessages["ForceExactChange"][currentLanguage]);
        if (GameStateManager.ForceRequireChange) activeMessages.Add(flagMessages["ForceRequireChange"][currentLanguage]);
        if (GameStateManager.AllowMischarge) activeMessages.Add(flagMessages["AllowMischarge"][currentLanguage]);
    }

    void OnGUI()
    {
        GUIStyle guiStyle = new();
        guiStyle.fontSize = 14;

        int yOffset = 0; // Vertical offset for each message
        foreach (string message in activeMessages)
        {
            GUI.Label(new(10, 10 + yOffset, 300, 50), message, guiStyle);
            yOffset += 20; // Increase the offset for the next message
        }
    }
}