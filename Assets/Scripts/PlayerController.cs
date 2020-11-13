using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : CardGameController, IDamageable, ITargetable, IBoostable
{
    public event Action<AbilityCard> SelectedAbilityCard = delegate { };
    public event Action<BoostCard> SelectedBoostCard = delegate { };
    public event Action EndedSelection = delegate { };
    public event Action ActionEnd = delegate { };

    Coroutine _abilityRoutine = null;
    Coroutine _boostRoutine = null;

    public override void PlayAbilityCard(int index)
    {
        AbilityCard targetCard = Hand.GetCard(index);
        if (targetCard != null && Actions > 0 && Actions >= targetCard.Cost)
        {
            if (_abilityRoutine == null) 
                _abilityRoutine = StartCoroutine(AbilityCardSelection(targetCard, index));
        }
    }

    public void StartBoostRoutine()
    {
        if (BoostDeck.TopItem != null && Actions > 0)
        {
            if (_boostRoutine == null)
                _boostRoutine = StartCoroutine(BoostCardSelection());
        }
    }

    public override void PlayBoostCard()
    {
        base.PlayBoostCard();
        EndedSelection?.Invoke();
    }

    // TODO I don't think it's best that this goes here, but I don't know a better way to organize it yet
    // TODO these if checks are kinda disgusting
    IEnumerator AbilityCardSelection(AbilityCard card, int index)
    {
        SelectedAbilityCard?.Invoke(card);
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = GetPointerRaycast();
                BoardSpace space = hit.collider?.GetComponent<BoardSpace>();
                if (space != null) // && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerSpace")
                {
                    if (!space.UseCard(card))
                    {
                        EndedSelection?.Invoke();
                        _abilityRoutine = null;
                        yield break;
                    } 
                    Actions -= card.Cost;
                    Hand.Remove(index);
                    AbilityDiscard.Add(card);

                    RaiseActions(Actions);
                    RaiseDiscard(AbilityDiscard);
                    RaiseHand(Hand);
                    EndedSelection?.Invoke();

                    _abilityRoutine = null;
                    yield break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EndedSelection?.Invoke();
                _abilityRoutine = null;
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator BoostCardSelection()
    {
        SelectedBoostCard?.Invoke(BoostDeck.TopItem);
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = GetPointerRaycast();
                IBoostable boostable = hit.collider?.GetComponent<IBoostable>();
                if (boostable != null)
                {
                    ITargetable boostTarget = boostable as ITargetable;
                    TargetController.CurrentTarget = boostTarget;
                    PlayBoostCard();

                    _boostRoutine = null;
                    yield break;
                }
                else if (hit.collider?.gameObject.layer == LayerMask.NameToLayer("PlayerTarget"))
                {
                    TargetController.CurrentTarget = GetComponent<ITargetable>();
                    PlayBoostCard();

                    EndedSelection?.Invoke();
                    _boostRoutine = null;
                    yield break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EndedSelection?.Invoke();
                _boostRoutine = null;
                yield break;
            }
            yield return null;
        }
    }

    private RaycastHit GetPointerRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity)) { }
        return hitInfo;
    }
}
