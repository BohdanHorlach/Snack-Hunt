using UnityEngine;


public class ObjectGrabber : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _itemFinder;
    [SerializeField] private IKHandAnimation[] _handIKAnimations;
    [SerializeField] private Transform _holdingPlace;

    private Transform _foundItem;
    private bool _isCanGrab => _foundItem != null;

    [HideInInspector] public Item Item;
    public bool IsHoldingObject => Item != null;


    private void OnEnable()
    {
        _itemFinder.OnObjectDetect += FindObject;
        _itemFinder.OnObjectLost += LostObject;
    }


    private void OnDisable()
    {
        _itemFinder.OnObjectDetect -= FindObject;
        _itemFinder.OnObjectLost -= LostObject;
    }


    private void Grab(Item item)
    {
        foreach (IKHandAnimation hand in _handIKAnimations)
            hand.TakeObject(item.HoldingInfo);
    }


    private void DropFromHands()
    {
        foreach (IKHandAnimation hand in _handIKAnimations)
            hand.DropObject();
    }


    private void EnableRigidbody(Rigidbody item)
    {
        item.isKinematic = false;
        item.useGravity = true;
    }


    private void DisableRigidbody(Rigidbody item)
    {
        item.isKinematic = true;
        item.useGravity = false;
    }


    private void FindObject(Transform item)
    {
        if (item != null)
            _foundItem = item;
    }


    private void LostObject(Transform item)
    {
        if (item == _foundItem)
            _foundItem = null;
    }


    public void AddItemToHoldingSpace()
    {
        Item.transform.SetParent(_holdingPlace);
        Item.transform.localPosition = _handIKAnimations[0].HoldingObject.HoldingTransform.Position;
        Item.transform.localRotation = _handIKAnimations[0].HoldingObject.HoldingTransform.Rotation;
    }



    public bool TryTakeObject()
    {
        if (_isCanGrab == false || Item != null)
            return false;

        Item = _foundItem.GetComponent<Item>();
        Grab(Item);
        DisableRigidbody(Item.Rigidbody);

        return true;
    }



    public void SetNewPlace(Transform newPlace)
    {
        DropFromHands();
        Item.transform.SetParent(newPlace);
    }


    public void DropObject()
    {
        SetNewPlace(null);
        EnableRigidbody(Item.Rigidbody);
        Item = null;
    }
}
