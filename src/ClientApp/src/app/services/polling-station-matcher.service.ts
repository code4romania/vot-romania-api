import { Injectable } from '@angular/core';
import { AssignedAddress, PollingStation } from './data.service';
import { Address } from './here-address.service';

@Injectable({providedIn: 'root'})
export class PollingStationMatcherService {
  private integralStreetToken = 'integral#';
  private oddNumbersToken = 'numere impare nr.';
  private evenNumbersToken = 'numere pare nr.';
  private numbersSeparatorToken = '-';
  private houseNumbersSeparatorToken = '#';

  findPollingStation(pollingStations: PollingStation[], userAddress: Address): PollingStation[] {
    return pollingStations.filter(pollingStation => {
      const assignedAddresses = pollingStation.assignedAddresses;
      const matchedAssignedAddress = assignedAddresses.find((assignedAddress) =>
        this.isSameLocality(assignedAddress, userAddress) &&
        this.isSameStreet(assignedAddress, userAddress) &&
        this.isMatchingNumber(assignedAddress, userAddress));
      return matchedAssignedAddress != null;
    });
  }

  private isSameLocality(assignedAddress: AssignedAddress, userAddress: Address): boolean {
    const assignedAddressLocality = this.normalize(assignedAddress.locality.toLowerCase());
    const userAddressCity = this.normalize(userAddress.city.toLowerCase());
    return assignedAddressLocality.includes(userAddressCity);
  }

  private isSameStreet(assignedAddress: AssignedAddress, userAddress: Address): boolean {
    const userStreet = this.normalize(userAddress.street.toLowerCase());
    const assignedAddressStreet = this.normalize(assignedAddress.street.toLowerCase());
    return userStreet.includes(assignedAddressStreet) || assignedAddressStreet.includes(userStreet);
  }

  private isMatchingNumber(assignedAddress: AssignedAddress, userAddress: Address): boolean {
    if (assignedAddress.houseNumbers === this.integralStreetToken) {
      return true;
    }

    if (!userAddress.houseNumber) {
      return true;
    }

    const assignedAddressHouseNumbers = assignedAddress.houseNumbers.split(this.houseNumbersSeparatorToken);
    const matchedHouseNumber = assignedAddressHouseNumbers
      .find(assignedAddressesHouseNumber => this.isHouseNumberMatch(assignedAddressesHouseNumber, userAddress.houseNumber));
    return matchedHouseNumber != null;
  }

  private normalize(str: string) {
    return str.normalize('NFKD').replace(/[^\w]/g, '');
  }

  private isHouseNumberMatch(assignedAddressesHouseNumber: string, houseNumberAsString: string) {
    const houseNumber = +houseNumberAsString;
    if (assignedAddressesHouseNumber.includes(this.evenNumbersToken)) {
      return this.isMatchingEvenNumber(assignedAddressesHouseNumber, houseNumber);
    }

    if (assignedAddressesHouseNumber.includes(this.oddNumbersToken)) {
      return this.isMatchingOddNumber(assignedAddressesHouseNumber, houseNumber);
    }

    return false;
  }

  private isMatchingEvenNumber(assignedAddressesHouseNumber: string, houseNumber: number) {
    if (houseNumber % 2 !== 0) {
      return false;
    }

    const numberRange = assignedAddressesHouseNumber.split(this.evenNumbersToken)[1];
    const lowerLimit = +numberRange.split(this.numbersSeparatorToken)[0].trim();
    const upperLimit = +numberRange.split(this.numbersSeparatorToken)[1].trim();

    return lowerLimit <= houseNumber && houseNumber <= upperLimit;
  }

  private isMatchingOddNumber(assignedAddressesHouseNumber: string, houseNumber: number) {
    if (houseNumber % 2 === 0) {
      return false;
    }

    const numberRange = assignedAddressesHouseNumber.split(this.oddNumbersToken)[1];
    const lowerLimit = +numberRange.split(this.numbersSeparatorToken)[0].trim();
    const upperLimit = +numberRange.split(this.numbersSeparatorToken)[1].trim();
    return lowerLimit <= houseNumber && houseNumber <= upperLimit;
  }
}
