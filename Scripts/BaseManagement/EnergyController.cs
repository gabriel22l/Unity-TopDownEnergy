using UnityEngine;
using System.Collections.Generic;
using System;

public class EnergyController: MonoBehaviour
{
      [SerializeField] private float tickDuration = 1;
      private float tickTimer;
      private float currentEnergy;
      private float maxEnergy;
      
      public float CurrentEnergy => currentEnergy;
      public float MaxEnergy => maxEnergy;
      public event Action OnEnergyChanged;

      private List<EnergyProducer> producers =  new List<EnergyProducer>();
      private List<EnergyStorage> storageContainers =  new List<EnergyStorage>();
      private List<EnergyConsumer> consumers =  new List<EnergyConsumer>();

      private void Update()
      {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickDuration)
            {
                  tickTimer -= tickDuration;
                  RunEnergySystem();
            }
      }
      public void RegisterEnergyProducer(EnergyProducer producer)
      {
            if (!producers.Contains(producer))
            {
                  producers.Add(producer);
                  maxEnergy += producer.EnergyStorage;
            }
      }
      public void RegisterEnergyStorage(EnergyStorage storage)
      {
            if (!storageContainers.Contains(storage))
            {
                  storageContainers.Add(storage);
                  maxEnergy += storage.storageAmount;
            }
      }
      public void RegisterEnergyConsumer(EnergyConsumer consumer)
      {
            if (!consumers.Contains(consumer))
            {
                  consumers.Add(consumer);
            }
      }
      private void RunEnergySystem()
      {
            foreach (EnergyProducer producer in producers)
            {
                  currentEnergy += producer.ProduceEnergy();
                  currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            }
            foreach (EnergyConsumer consumer in consumers)
            {
                  if(!consumer.IsConsuming) continue;
                  if (currentEnergy >= consumer.ConsumptionPerTick)
                  {
                        currentEnergy -= consumer.ConsumptionPerTick;
                        consumer.Power();
                  }
                  else
                  {
                        consumer.UnPower();
                  }
            }
            OnEnergyChanged?.Invoke();
      }
      public bool RemoveEnergy(float amount)
      {
            if(currentEnergy < amount) return false;
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke();
            return true;
      }
}