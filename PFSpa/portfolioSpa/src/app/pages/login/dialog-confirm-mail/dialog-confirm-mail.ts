import { Component, ElementRef, QueryList, ViewChildren, inject, signal, AfterViewInit, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import {
    MAT_DIALOG_DATA,
    MatDialogActions,
    MatDialogContent,
    MatDialogRef,
    MatDialogTitle
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

export interface DialogData {
    email: string;
    expTime: Date;
}



@Component({
    selector: 'dialog-confirm-mail',
    templateUrl: './dialog-confirm-mail.html',
    styleUrls: ['./dialog-confirm-mail.scss'],
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatDialogTitle,
        MatDialogContent,
        MatDialogActions,
        TranslateModule
    ],
})
export class EmailConfirmDialog implements AfterViewInit {
    readonly dialogRef = inject(MatDialogRef<EmailConfirmDialog>);
    
    readonly data = inject<DialogData>(MAT_DIALOG_DATA);
    readonly expTime = this.data.expTime;
    readonly email = this.data.email;
    
    expiryMinutes: string = '0';
    expirySeconds: string = '0';
    
    // Reactive signal for OTP digits
    readonly otpDigits = signal<string[]>(['', '', '', '', '', '']);
    
    @ViewChildren('otpInput') otpInputs!: QueryList<ElementRef<HTMLInputElement>>;
    
    @Output() verifyCode = new EventEmitter<string>();

    constructor() {
        
        this.convertExpiryTime();
        
        // Update expiry time every second
        setInterval(() => {
            this.convertExpiryTime();
        }, 1000);
        
    }
    
    ngAfterViewInit(): void {
        
        
        
        // Focus on the first input when the view is initialized
        this.otpInputs.first?.nativeElement.focus();
    }
    
    
    onOtpInput(event: Event, index: number): void {
        const input = event.target as HTMLInputElement;
        let value = input.value.trim();
        
        // Accept only single digits
        if (!/^\d$/.test(value)) {
            input.value = '';
            return;
        }
        
        // Update only the modified cell
        this.otpDigits.update(digits => {
            const copy = [...digits];
            copy[index] = value;
            return copy;
        });
        
        // Pass to the next input
        const next = input.parentElement?.querySelectorAll('input')[index + 1] as HTMLInputElement;
        next?.focus();
    }
    
    
    onOtpKeyDown(event: KeyboardEvent, index: number): void {
        const input = event.target as HTMLInputElement;
        
        // Backspace
        if (event.key === 'Backspace') {
            event.preventDefault();
            
            // Clear current cell
            this.otpDigits.update(digits => {
                const copy = [...digits];
                copy[index] = '';
                return copy;
            });
            
            // Move to previous input
            const prev = input.parentElement?.querySelectorAll('input')[index - 1] as HTMLInputElement;
            prev?.focus();
            return;
        }
    }
    
    
    onPaste(event: ClipboardEvent): void {
        event.preventDefault();
        
        // Get pasted data
        const input = event.target as HTMLInputElement;
        const data = event.clipboardData?.getData('text') ?? '';
        
        // Extract digits and fill the inputs
        const digits = data.replace(/\D/g, '').slice(0, 6).split('');
        
        // Fill the inputs with the extracted digits
        this.otpDigits.set([...digits, ...Array(6 - digits.length).fill('')]);
        
        // Focus the next empty input or the last one
        const lastFilledIndex = digits.length > 0 ? digits.length - 1 : 0;
        const focusIndex =
        digits.length < this.otpDigits().length
        ? lastFilledIndex + 1
        : lastFilledIndex;
        
        const next = input.parentElement?.querySelectorAll('input')[focusIndex] as HTMLInputElement;
        next?.focus();
    }
    
    onOtpFocus(event: FocusEvent): void {
        // Select all text on focus
        event.preventDefault();
        
        const input = event.target as HTMLInputElement;
        
        input.select();
    }
    
    
    
    confirmVerification(): void {
        
        if(this.otpDigits().some(digit => digit === '')) {
            // If any digit is missing, do nothing (or show an error)
            return;
        }

        // emit signal with the complete OTP code
        this.verifyCode.emit(this.otpDigits().join(''));
        
        // Close the dialog and return the code
        //this.dialogRef.close(this.otpDigits().join(''));
    }
    
    convertExpiryTime(): void {
        const now = new Date();
        const expTime = new Date(this.expTime);
        
        const diffMs = expTime.getTime() - now.getTime();
        
        // Calcolo minuti e secondi rimanenti
        const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((diffMs % (1000 * 60)) / 1000);
        
        // Conversione in stringa con zero iniziale
        this.expiryMinutes = minutes.toString().padStart(2, '0');
        this.expirySeconds = seconds.toString().padStart(2, '0');
    }

    disabledConfirm(): boolean {
        return this.otpDigits().some(digit => digit === '');
    }
    
}
