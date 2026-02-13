using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
public static GameController Instance { get; private set; }

    public TextMeshProUGUI texteStatus;
    public GameObject gameOverPanel;
    public TextMeshProUGUI textGagnant;
    
    public GameObject symboleX;
    public GameObject symboleO;

    private bool tourJoueurX = true;
    private int[] caseTableau = new int[9];
    private bool jeuActive = false;
    
    private PlayerInputActions inputActions;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        inputActions = new PlayerInputActions();
    }
    
    void OnEnable()
    {
        inputActions.Players.Touch.performed += OnInputClick;
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Players.Touch.performed -= OnInputClick;
        inputActions.Disable();
    }
    
    private void OnInputClick(InputAction.CallbackContext context)
    {
        if (!jeuActive) return;

        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("TicTacToeCase"))
                {
                    // Utilisation du nom de la Case (1, 2, 3, 4) et le mais en Index (Trouver avec l'IA) Prompt : En C# je peux prendre un string qui est un nombre 1 2 3 etc et le mettre en int ? 
                    if (int.TryParse(hit.collider.name, out int index))
                    {
                        JouerUnTour(index, hit.collider.transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Commencer le Jeu (X Commence Tout le Temps)
    /// </summary>
    public void CommencerJeu()
    {
        jeuActive = true;
        texteStatus.text = "Tour du joueur X";
    }

    /// <summary>
    /// Permet de Joueur un Tour
    /// </summary>
    /// <param name="index">Index de la Case (1, 2, 3, etc)</param>
    /// <param name="caseDeJeu">Tableau de Int pour Vérifier la Condition de Victoire</param>
    public void JouerUnTour(int index, Transform caseDeJeu)
    {
        Debug.Log(caseDeJeu.name);
        if (!jeuActive || caseTableau[index] != 0) return;
        Debug.Log(caseTableau[index]);

        caseTableau[index] = tourJoueurX ? 1 : 2;
        Instantiate(tourJoueurX ? symboleX : symboleO, caseDeJeu.position, caseDeJeu.rotation, caseDeJeu);

        if (ConditionVictoire()) FinDuJeu(tourJoueurX ? "X a gagné !" : "O a gagné !");
        else if (TableauPlein()) FinDuJeu("Match Nul !");
        else
        {
            tourJoueurX = !tourJoueurX;
            texteStatus.text = tourJoueurX ? "Tour de X" : "Tour de O";
        }
    }

    /// <summary>
    /// Regarde la Condition de Victoire (Générer avec l'IA Gemini) Prompt = Dans un jeux de Tic Tac Toe en C# comment je pourrais faire pour regarder les conditions de Victoire
    /// </summary>
    /// <returns>Vrai si le Jeu est Gagner et Faux si le jeux est pas gagner</returns>
    private bool ConditionVictoire()
    {
        int[,] winPatterns = { {0,1,2}, {3,4,5}, {6,7,8}, {0,3,6}, {1,4,7}, {2,5,8}, {0,4,8}, {2,4,6} };
        for (int i = 0; i < 8; i++)
        {
            if (caseTableau[winPatterns[i,0]] != 0 &&
                caseTableau[winPatterns[i,0]] == caseTableau[winPatterns[i,1]] &&
                caseTableau[winPatterns[i,1]] == caseTableau[winPatterns[i,2]]) return true;
        }
        return false;
    }

    /// <summary>
    /// Regarde si le Tableau est plein après chaques tours pour vérifier les Parties Nulles
    /// </summary>
    /// <returns>Faux si Vide, Vrai si Plein</returns>
    private bool TableauPlein() { foreach (int val in caseTableau) if (val == 0) return false; return true; }

    /// <summary>
    /// Fin du Jeu
    /// </summary>
    /// <param name="message">Message de Victoire du Joueur ou Message de Partie Nulle, Active aussi le GameOverPanel</param>
    private void FinDuJeu(string message)
    {
        jeuActive = false;
        textGagnant.text = message;
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Méthode pour Rejouer, 
    /// </summary>
    public void Rejouer()
    {
        caseTableau = new int[9];
        tourJoueurX = true;
        jeuActive = true;
        gameOverPanel.SetActive(false);
        texteStatus.text = "Tour du joueur X";
        
        // Trouver Avec Gemini
        GameObject[] cases = GameObject.FindGameObjectsWithTag("TicTacToeCase");
        foreach (GameObject c in cases)
        {
            foreach (Transform child in c.transform) Destroy(child.gameObject);
        }
        // Fin du Code Trouver avec Gemini
    }
}
