using System.Collections.Generic;
using UnityEngine;

public enum PooledPrefabName
{
    //It's faster to lookup by enum then a string tag, so make a matching enum for your objectPoolItem
    Bullet = 0,
    FootstepDust = 1,
    //etc, etc..
}
namespace Stahle.Utility
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        [HideInInspector]
        public Transform objectPoolName; //set automatically depending on asset name, must be public

        [Header("Select the name of the object you're pooling")]
        [Tooltip("Go into this script and make a name that matches your item if not here.")]
        public PooledPrefabName prefabName; //user drags into inspector

        [Header("How many copies to make of this prefab?")]
        public int amountToPool; //user drags into inspector

        [Header("Drag prefabs you want to clone here")]
        public GameObject objectToPool; //user drags into inspector

        [Header("Should the list of clones expand? Or is there a hard cap?")]
        public bool shouldExpand = true;
    }
    //Pooling is pre-instantiating all objects that would ever exist at once in a scene!
    public class ObjectPooler : PersistantSingleton<ObjectPooler>
    {
        #region Singleton
        //public static ObjectPooler SharedInstance = null;

        //private void Awake()
        //{
        //    //If there is not already an instance of ObjectPooler, set it to this
        //    if (SharedInstance == null)
        //    {
        //        SharedInstance = this;
        //    }
        //    //else If there is already an instance of ObjectPooler, destroy it because there can only be one highlander.
        //    else if (SharedInstance != this)
        //    {
        //        print("there was already an instance of ObjectPooler, destroying this");
        //        Destroy(this.gameObject);
        //    }
        //    //Make the ObjectPooler not destroyed when scenes change or reload.
        //    DontDestroyOnLoad(gameObject);
        //}
        #endregion
        public List<ObjectPoolItem> itemsToPool; //the individual prefabs we want to pool
        public List<GameObject> pooledObjects; //all of the pooled objects in one list
        private const string PARENT = "Parent";

        private void Start()
        {
            //create a new list of GameObjects
            pooledObjects = new List<GameObject>();
            //For each different prefab we are pooling
            foreach (ObjectPoolItem item in itemsToPool)
            {
                //Make empty parent to stay organized
                GameObject parent = new GameObject();
                //child that parent to this
                parent.transform.SetParent(transform);
                //set its name to the thing we are pooling + Parent
                parent.name = item.objectToPool.name + PARENT;
                //set the item's poolName to it
                item.objectPoolName = parent.transform;
                //loop amountToPool many times - say we want 20 bullets
                for (int i = 0; i < item.amountToPool; i++)
                {
                    //make an instance of that prefab we want - the bullet
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    //deactivate it - Start() or OnEnable() won't be called yet
                    obj.SetActive(false);
                    //add that bullet to the List
                    pooledObjects.Add(obj);
                    //child object to its parent to organize
                    obj.transform.SetParent(parent.transform);
                }
            }
        }
        //gets a pooled object by enum
        public GameObject GetPooledObject(PooledPrefabName poolPrefabName)
        {
            //go through the types of prefabs we pooled - Bullet, Explosion
            foreach (ObjectPoolItem item in itemsToPool)
            {
                //if we found an enum match
                if (item.prefabName == poolPrefabName)
                {
                    int actives = 0;
                    //go through its parents' children
                    foreach (Transform t in item.objectPoolName)
                    {
                        //find one that is inactive - meaning it can be used now
                        if (!t.gameObject.activeInHierarchy)
                        {
                            //and use that one
                            return t.gameObject;
                        }
                        //else we found one that was active
                        actives++;
                    }
                    //if we have gone through all the kids and they were all active
                    if (actives == item.objectPoolName.childCount)
                    {
                        //and we are allowed to expand our list...
                        if (item.shouldExpand)
                        {
                            //then make another prefab clone to add to the pool
                            GameObject obj = (GameObject)Instantiate(item.objectToPool);
                            //setting it to false right away prevents it's Start() or OnEnable from running
                            obj.SetActive(false);
                            //add that object to the list of objects we have
                            pooledObjects.Add(obj);
                            //and return it to whoever called this function
                            return obj;
                        }
                    }
                }
            }
            return null; //no prefab name of that exists
        }
        //gets a pooled object by tag
        public GameObject GetPooledObject(string tag)
        {
            //go through the types of prefabs we pooled - Bullet, FootstepDust..
            foreach (ObjectPoolItem item in itemsToPool)
            {
                //if we found a string tag match
                if (item.objectToPool.CompareTag(tag))
                {
                    int actives = 0;
                    //go through its parents' children
                    foreach (Transform t in item.objectPoolName)
                    {
                        //find one that is inactive - meaning it can be used now
                        if (!t.gameObject.activeInHierarchy)
                        {
                            //and use that one
                            return t.gameObject;
                        }
                        //else we found one that was active
                        actives++;
                    }
                    //if we have gone through all the kids and they were all active
                    if (actives == item.objectPoolName.childCount)
                    {
                        //and we are allowed to expand our list...
                        if (item.shouldExpand)
                        {
                            //then make another prefab clone to add to the pool
                            GameObject obj = (GameObject)Instantiate(item.objectToPool);
                            //setting it to false right away prevents it's Start() or OnEnable from running
                            obj.SetActive(false);
                            //add that object to the list of objects we have
                            pooledObjects.Add(obj);
                            //and return it to whoever called this function
                            return obj;
                        }
                    }
                }
            }
            return null; //no prefab name of that exists
        }
        // todo compare performance by big 0 unit testing 
        //Gets a pooledObject by tag
        //public GameObject GetPooledObject(string tag)
        //{
        //    //go through all of our pooled objects, explosions, asteroids, bullets, etc
        //    for (int i = 0; i < pooledObjects.Count; i++)
        //    {
        //        //If i find one that is inactive and has the same tag as the one you requested
        //        if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
        //        {
        //            //then return the inactive go with the matching tag
        //            return pooledObjects[i];
        //        }
        //    }
        //    //if we didn't return something, then nothing matched the tag we want or we have none available!

        //    //go through the prefabs we want to pool
        //    foreach (ObjectPoolItem item in itemsToPool)
        //    {
        //        //if it has the tag we are searching for
        //        if (item.objectToPool.CompareTag(tag))
        //        {
        //            //if we should instantiate() to expand the list of pooled objects
        //            if (item.shouldExpand)
        //            {
        //                //make another one
        //                GameObject obj = (GameObject)Instantiate(item.objectToPool);
        //                obj.SetActive(false);
        //                //add it to list of crap
        //                pooledObjects.Add(obj);
        //                return obj;
        //            }
        //        }
        //        else
        //        {
        //            print("Can't create an additional prefab clone bc the tag you searched for doesn't exist!");
        //            break;
        //        }
        //    }
        //    return null;
        //}
    } // end class
} //end namespace