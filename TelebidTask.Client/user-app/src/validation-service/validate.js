export const validateName = (name) => {
    if(name.trim() === '')
        return 'Name cannot be empty!';

    if(name[0].toUpperCase() !== name[0]){
        if(!containsOnlyLetters(name)){
            return 'First letter of name must be uppercase and the name must contain only letters!';
        }
        return 'First letter of name must be uppercase!';
    }

    if(!containsOnlyLetters(name)){
        return 'Name should contain only letters';
    }

    return ''
}

export const validateEmail = (email) => {
    if(email.length === 0){
        return 'Email must be provided!';
    }

    var pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    if(!email.match(pattern)){
        return 'This email is not valid!'
    }

    return '';
}

export const validatePassword = (password) => {
    if(password.length === 0){
        return 'Password must be provided!';
    }

    var pattern = /^(?=.*[0-9])(?=.*[A-Z])(?=.*[!@#$%^&*.\-_\\\/<>;,|\(\)`~\{\}=])([a-zA-Z0-9!@#$%^&*.\-_\\\/<>;,|\(\)`~\{\}=]){6,}$/

    if(!pattern.test(password)){
        return 'Password should be 6+ characters long and it should contain at least one uppercase letter, number and special character!'
    }

    return '';
}

const containsOnlyLetters = (value) => {
    return /^[a-zA-Z]+$/.test(value);
}