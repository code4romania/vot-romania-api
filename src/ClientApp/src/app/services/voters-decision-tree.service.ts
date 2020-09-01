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


  getDecisionTree$(): Observable<OperatorDecisionTree> {
    // TODO: put it here
    const data: OperatorDecisionTree = {

      '0': {
        id: '0',
        label: 'Esti cetatean roman?',
        options: [
          '00',
        ]
      },
      '00': {
        id: '00',
        text: 'nu poti vota!'
      },
      '01': {
        id: '01',
        votersOptionId: 5
      },

      '1': {
        id: '1',
        label: 'Esti cetatean UE?',
        options: [
          '01'
        ]
      },



      'initial': {
        id: 'initial',
        options: [
          '0',
          '1'
        ]
      }
    };

    return of(data);
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

  currentSentence$: Observable<string> = combineLatest(
    this.tree$,
    this.state$
  ).pipe(
    filter(([tree]) => treeIsErrorFree(tree)),
    map(([tree, { previousBranchIds }]) =>
      isInitialDecision(previousBranchIds)
        ? 'Incepe prin a alege una din optiunile de mai jos'
        : `${previousBranchIds
          .map(entityId => {
            return tree[entityId].label;
          })
          .join(' ')}...`.trim()
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

  constructor() { }

  private get snapShot(): State {
    return this.state$.getValue();
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
