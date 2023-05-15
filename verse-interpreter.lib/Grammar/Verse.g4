parser grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( program ) * EOF;

declaration : ID ':' type 
            | ID ':=' (INT | expression) 
            | ID ':' type
            | ID ':=' string_rule
            ;

constructors : type_constructor
             ;

program : declaration program
        | function_definition program
        | function_call program
        | type_header program
        | type_member_access program
        | type_member_definition program
        | constructors program
        | (NEWLINE | NEWLINE NEWLINE) program
        | program ';' program
        | declaration
        | function_call
        | function_definition
        | expression
        | type_header
        | type_member_access
        | type_member_definition
        | constructors
        ;

block : declaration
      | function_call
      | expression
      | declaration block
      | function_call block
      | expression block
      ;

// Functions

function_call : ID '(' param_call_item ')'
              | ID '(' ')'
              ;

function_definition : ID function_param ':' INTTYPE '=' function_body
                     ;

function_body : NEWLINE INDENT block function_body
              | block ';' block
              | block
              ;

function_param : '(' ')'
               | '(' param_def_item ')'
               ;
               
param_def_item     : declaration
                   | declaration ',' param_def_item
                   ;
                   
param_call_item : (INT | ID | function_call)
                | (INT | ID | function_call) ',' param_call_item
                ;

// Type definition

type_constructor : ID ':=' ID '('')'
                 ;

type_header : DATA ID '=' ID NEWLINE '{' type_body NEWLINE '}'
            ;
            
type_body : NEWLINE INDENT declaration
          | NEWLINE INDENT declaration type_body
          ;
   
type_member_definition : type_member_access '=' (string_rule | INT)
                       ;

          
type_member_access : ID'.'ID
                   ;

// Strings
string_rule : '"' ~'"'* '"'
            ;
// Conditionals

if_block    : 'if' '(' comp_expression ')' 'then' 'else' ;

comp_expression
    : comp_term
    | comp_expression comparsion_op comp_term 
    ;

comp_term
    : comp_factor
    | comp_term comparsion_op factor
    ;

comp_factor
    : comp_primary
    | comparsion_op comp_factor
    ;

comp_primary
    : ID
    | INT
    | '(' comp_expression ')'
    ;


// Math expression rules
expression
    : term
    | expression operator term 
    ;

term
    : factor
    | term operator factor
    ;

factor
    : primary
    | operator factor
    ;

primary
    : ID
    | INT
    | type_member_access
    | '(' expression ')'
    ;

type : (INTTYPE | STRINGTYPE ) ;
comparsion_op : ('>' | '<' | '|' | '=' )   ; 
operator : ('*' | '/' |'-'|'+'| '>' | '|');