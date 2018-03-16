using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    private GameObject _mineObject;
    private ParticleSystem _explosionEffect;
    public GameObject _minedTile;

    void Awake()
    {
        _mineObject = transform.GetChild(0).gameObject;
        _explosionEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
    }
    
    private IEnumerator Explode()
    {
        _explosionEffect.Play(true);

        yield return new WaitForSeconds(1f);

        _mineObject.SetActive(false);

        if (_minedTile != null)
        {
            _minedTile.SetActive(false);
        }

        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }

    public void Expolde()
    {
        StartCoroutine(Explode());
    }

    /// <summary>
    /// Set mined tile
    /// </summary>
    /// <param name="tile">Tile</param>
    /// <param name="isVisible">Is mine visible for the player</param>
    /// <param name="setPosition">Set the same position of the mine as tile</param>
    public void SetMineTile(CubeScript tile, bool isVisible, bool setPosition = true)
    {
        _mineObject.SetActive(isVisible);

        _minedTile = tile.gameObject;

        if (setPosition)
        {
            transform.position = _minedTile.transform.position;
        }
    }
}

