import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';
import { AngularEditorConfig } from '@kolkov/angular-editor';

@Component({
  selector: 'app-voters-option-editor',
  templateUrl: './voters-option-editor.component.html',
  styleUrls: ['./voters-option-editor.component.scss']
})
export class VotersOptionEditorComponent implements OnInit {
  form: FormGroup;

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
  
  constructor(
    public dialogRef: MatDialogRef<VotersOptionEditorComponent>,
    // @Inject(MAT_DIALOG_DATA) public data: {}
  ) {
    this.dialogRef.disableClose = true;
    this.form = new FormGroup({
      title: new FormControl('', Validators.required),
      description: new FormControl('', Validators.required),
    });

    // if (data) {
    //   this.form.patchValue(data.pollingStation);
    // }

  }

  ngOnInit() {

  }

  onSubmit() {

    this.dialogRef.close(this.form.value);
  }

  onClose(): void {
    this.dialogRef.close();
  }
} 
