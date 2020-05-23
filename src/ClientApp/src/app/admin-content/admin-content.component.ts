import {Component, OnInit} from '@angular/core';
import {VotingGuide} from '../services/data.service';
import {AngularEditorConfig} from '@kolkov/angular-editor';
import {select, Store} from '@ngrx/store';
import {ApplicationState} from '../state/reducers';
import {getGeneralInfo, getVotingGuide} from '../state/selectors';

@Component({
  selector: 'app-admin-content',
  templateUrl: './admin-content.component.html',
  styleUrls: ['./admin-content.component.scss']
})
export class AdminContentComponent implements OnInit {
  public generalInfo: string;
  public votingGuide: VotingGuide;
  public editing = {};
  public items = [];
  public editorConfig: AngularEditorConfig = {
    editable: true,
    toolbarHiddenButtons: [
      [
        'subscript',
        'superscript',
        'justifyLeft',
        'justifyCenter',
        'justifyRight',
        'justifyFull',
        'indent',
        'outdent',
        'heading',
        'fontName'
      ],
      [
        'textColor',
        'backgroundColor',
        'customClasses',
        'insertImage',
        'insertVideo',
        'insertHorizontalRule',
      ]
    ]
  };

  constructor(private store: Store<ApplicationState>) {
  }

  ngOnInit() {
    this.store.pipe(select(getGeneralInfo)).subscribe(value => {
      this.generalInfo = value;
      this.items[0] = {field: this.generalInfo, editing: 'generalInfo', title: 'General info'};
    });

    this.store.pipe(select(getVotingGuide)).subscribe(value => {
      this.votingGuide = value || {
        description: '',
        options: []
      };
      this.items[1] = {field: this.votingGuide.description, editing: 'votingGuide.description', title: 'Voting guide'};
      this.votingGuide.options.forEach((option, idx) => {
        this.items[2 * (idx + 1)] = {field: option.title, editing: `option${idx}Title`, title: `Option ${idx + 1} title`};
        this.items[2 * (idx + 1) + 1] = {
          field: option.description,
          editing: `option${idx}Description`,
          title: `Option ${idx + 1} description`
        };
      });
    });
  }

}
