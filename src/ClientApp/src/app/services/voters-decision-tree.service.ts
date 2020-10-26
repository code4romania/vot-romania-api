import { Injectable } from '@angular/core';
import { Observable, of, BehaviorSubject, combineLatest } from 'rxjs';
import { catchError, shareReplay, filter, map, mapTo } from 'rxjs/operators';

export interface OperatorDecisionTree {
  [key: string]: OperatorTreeNode;
  initial: Required<Pick<OperatorTreeNode, 'id' | 'options'>>;
  error?: any;
}

export interface OperatorTreeNode {
  id: string;
  label?: string;
  options?: string[];
  votersOptionId?: number;
  text?: string;
}

export interface OperatorTreeNodeWithOptions extends OperatorTreeNode {
  options: string[];
}

export function isInitialDecision(previousBranchIds: string[]): boolean {
  return (
    previousBranchIds.includes('initial') && previousBranchIds.length === 1
  );
}

export function treeIsErrorFree(tree): boolean {
  return !tree.error;
}

export function nodeHasOptions(node): node is OperatorTreeNodeWithOptions {
  return !!node.options;
}

export interface State {
  previousBranchIds: string[];
  currentBranchId: string;
}


@Injectable({
  providedIn: 'root'
})
export class VotersDecisionTreeService {

  constructor() { }

  private get snapShot(): State {
    return this.state$.getValue();
  }

  private initialState: State = {
    previousBranchIds: ['initial'],
    currentBranchId: 'initial'
  };
  private state$ = new BehaviorSubject<State>(this.initialState);
  private tree$: Observable<
    OperatorDecisionTree
  > = this.getDecisionTree$().pipe(
    catchError(error => of(error)), // This helps if the JSON for some reason fails to get fetched
    shareReplay()
  );

  currentSentence$: Observable<string[]> = combineLatest(
    this.tree$,
    this.state$
  ).pipe(
    filter(([tree]) => treeIsErrorFree(tree)),
    map(([tree, { previousBranchIds }]) => {
      const value = isInitialDecision(previousBranchIds)
        ? ['votersGuide.callToAction']
        : previousBranchIds
          .map(entityId => {
            return tree[entityId].label;
          })
          .filter(val => val);

      return value;
    }
    )
  );

  options$: Observable<(OperatorTreeNode)[]> = combineLatest(
    this.tree$,
    this.state$
  ).pipe(
    filter(([tree, state]) => {
      return (
        treeIsErrorFree(tree) &&
        !!tree[state.currentBranchId] &&
        !!tree[state.currentBranchId].options
      );
    }),
    map(([tree, state]) => {
      // Project is currently using TypeScript 2.9.2
      // With TS 3.1+ this can be done better if we map to [tree, node] and typeguard with a tuple in a filter
      // filter((a): a is [OperatorDecisionTree, OperatorTreeNodeWithOptions] => !a[0].error && !!a[1].options)
      const node = tree[state.currentBranchId];
      return nodeHasOptions(node)
        ? node.options.map(option => tree[option])
        : tree['initial'].options.map(option => tree[option]);
    })
  );

  isBeyondInitialQuestion$: Observable<boolean> = this.state$.pipe(
    map(({ currentBranchId }) => currentBranchId !== 'initial')
  );

  // This helps if the JSON for some reason fails to get fetched
  hasError$ = this.tree$.pipe(
    filter(tree => !!tree.error),
    mapTo(true)
  );


  getDecisionTree$(): Observable<OperatorDecisionTree> {
    const data: OperatorDecisionTree = {
      'initial': {
        id: 'initial',
        options: [
          '0',
          '1'
        ]
      },
      '0': {
        id: '0',
        label: 'votersGuide.label0',
        options: [
          '00',
          '01'
        ]
      },
      '1': {
        id: '1',
        label: 'votersGuide.label1',
        options: [
          '10',
          '11'
        ]
      },
      '00': {
        id: '00',
        label: 'votersGuide.label00',
        options: [
          '000'
        ]
      },
      '01': {
        id: '01',
        label: 'votersGuide.label01',
        options: [
          '010',
          '011'
        ]
      },
      '000': {
        id: '000',
        text: 'votersGuide.label000'
      },
      '010': {
        id: '010',
        label: 'votersGuide.label010',
        options: [
          '0100'
        ]
      },
      '0100': {
        id: '0100',
        text: 'votersGuide.label0100'
      },
      '011': {
        id: '011',
        label: 'votersGuide.label011',
        options: [
          '0110',
          '0111'
        ]
      },
      '0110': {
        id: '0110',
        label: 'votersGuide.label0110',
        options: [
          '01100'
        ]
      },
      '01100': {
        id: '01100',
        text: 'votersGuide.label01100'
      },
      '0111': {
        id: '0111',
        label: 'votersGuide.label0111',
        options: [
          '01110',
          '01111'
        ]
      },
      '01110': {
        id: '01110',
        label: 'votersGuide.label01110',
        options: [
          '011100'
        ]
      },
      '011100': {
        id: '011100',
        text: 'votersGuide.label011100'
      },
      '01111': {
        id: '01111',
        label: 'votersGuide.label01111',
        options: [
          '011110'
        ]
      },
      '011110': {
        id: '011110',
        text: 'votersGuide.label011110',
      },
      '10': {
        id: '10',
        label: 'votersGuide.label10',
        options: [
          '100',
          '101'
        ]
      },
      '100': {
        id: '100',
        label: 'votersGuide.label100',
        options: [
          '1000'
        ]
      },
      '1000': {
        id: '1000',
        text: 'votersGuide.label110'
      },
      '101': {
        id: '101',
        label: 'votersGuide.label101',
        options: [
          '1010'
        ]
      },
      '1010': {
        id: '1010',
        text: 'votersGuide.label1010'
      },
      '11': {
        id: '11',
        label: 'votersGuide.label11',
        options: [
          '110',
        ]
      },
      '110': {
        id: '110',
        text: 'votersGuide.label1000'
      },
    };

    return of(data);
  }

  selectOption(optionId: string): void {
    this.state$.next({
      previousBranchIds: [...this.snapShot.previousBranchIds, optionId],
      currentBranchId: optionId
    });
  }

  back(): void {
    const previousOptionId = this.snapShot.previousBranchIds[
      this.snapShot.previousBranchIds.length - 2
    ];

    if (previousOptionId) {
      this.state$.next({
        previousBranchIds: [
          ...this.snapShot.previousBranchIds.slice(
            0,
            this.snapShot.previousBranchIds.length - 1
          )
        ],
        currentBranchId: previousOptionId
      });
    }
  }

  startOver(): void {
    this.state$.next(this.initialState);
  }
}
