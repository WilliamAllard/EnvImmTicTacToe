using UnityEngine;
using UnityEngine.XR.ARFoundation;

// Classe Créer avec l'aide de Gemini (Il ma proposer le placement par Événement de Détection et ses le seul qui marche)
public class ARPlacementManager : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject ticTacToePrefab;
    
    private GameObject grilleDeJeuGénérer;

    void OnEnable()
    {
        planeManager.trackablesChanged.AddListener(OnChange);
    }

    void OnDisable()
    {
        planeManager.trackablesChanged.RemoveListener(OnChange);
    }

    /// <summary>
    /// Regarde quand il y a un changement avec les planes 
    /// </summary>
    private void OnChange(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (grilleDeJeuGénérer == null && args.added.Count > 0)
        {
            ARPlane firstPlane = args.added[0];
            PlacerLaGrille(firstPlane);
        }
    }

    /// <summary>
    /// Placer la Grille
    /// </summary>
    /// <param name="plane">Les Murs</param>
    private void PlacerLaGrille(ARPlane plane)
    {
        grilleDeJeuGénérer = Instantiate(ticTacToePrefab, plane.center, Quaternion.identity);
        
        Vector3 lookPos = Camera.main.transform.position - grilleDeJeuGénérer.transform.position;
        lookPos.y = 0; 
        grilleDeJeuGénérer.transform.rotation = Quaternion.LookRotation(-lookPos);
        
        if (GameController.Instance != null)
        {
            GameController.Instance.CommencerJeu();
        }
    }
}
