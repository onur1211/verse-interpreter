parser grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( program ) * EOF;

declaration : ID ':' type
            | ID ':=' (value_definition | constructor_body)
            | ID '=' (value_definition | constructor_body)
            | ID ':=' array_literal
            | ID '=' array_literal
            ;


value_definition : INT
                 | ID 
                 | NOVALUE 
                 | expression 
                 | constructor_body 
                 | string_rule 
                 | array_literal 
                 | function_call 
                 | array_index 
                 | type_member_access 
                 | range_expression
                 ;
                 

program : function_definition program
        | for_rule program
        | declaration program
        | function_call program
        | type_header program
        | type_member_definition program
        | type_member_access program
        | if_block program
        | expression program
        | (NEWLINE | NEWLINE NEWLINE) program
        | program ';' program
        | declaration
        | function_call
        | type_header
        | type_member_definition
        | type_member_access
        | function_definition
        | expression
        | array_index
        | if_block
        | value_definition
        | for_rule
        ;

block : declaration
      | function_call
      | expression
      | if_block
      | for_rule
      ;

body : inline_body
     | NEWLINE spaced_body
     | bracket_body
     ;
              
inline_body : block ';' inline_body
            | block
            ;
            
spaced_body : INDENT* block
            | INDENT* block NEWLINE spaced_body
            ;


// Range (..) expressions
range_expression    : INT RANGE INT
                    | INT ',' INT RANGE INT
                    ;


// Arrays/Tuples
array_literal : '(' array_elements ')'
              | '('')'
              ;


array_elements : value_definition (',' array_elements)*
               | declaration (',' array_elements)*
               | ID (',' array_elements)*
               ;

array_index : ID '[' (INT|ID) ']' 
            | ID '[' declaration ']'
            ;
            
bracket_body : '{' block+ '}';

// Functions
function_call: ID '(' param_call_item ')'
             | ID '(' ')'
             ;

function_definition : ID function_param ':' type NEWLINE* '{' NEWLINE* body NEWLINE*'}' #func
                    | ID ':=' '(' param_def_item '=>' body ')' #lambdaFunc
                    ;

                     
function_param : '(' ')'
               | '(' param_def_item ')'
               ;
               
param_def_item  : declaration
                | declaration ',' param_def_item
                ;
                   
param_call_item: value_definition
               | ID
               | value_definition ',' param_call_item
               | ID ',' param_call_item
               ;

// For
for_rule    : 'for' '{'(NEWLINE)* for_body (NEWLINE)* '}'
            ;

for_body : choice_rule #choiceBody
         | narrowing_body #narrowingBody
         ;

narrowing_body : declaration ';' expression
               ;


// Type definition
constructor_body : INSTANCE ID '('')'
                 | INSTANCE ID '('param_def_item ')'
                 ;


type_header : DATA ID '=' ID NEWLINE '{' type_body NEWLINE '}'
            | DATA ID '=' ID '{' type_body '}'
            ;
            
type_body : NEWLINE INDENT declaration
          | NEWLINE INDENT declaration type_body
          | declaration ',' type_body
          | declaration
          ;
   
type_member_definition : type_member_access '=' value_definition
                       ;

          
type_member_access : type_member_access '.' ID
                   | ID'.'ID
                   ;

// Strings
string_rule : SEARCH_TYPE
            ;

choice_rule : value_definition multi_choice_rule
            ;

multi_choice_rule :
                  | '|' value_definition multi_choice_rule
                  ;

            
// Conditionals

if_block    : 'if' '(' expression ')' then_block else_block 
            ;

then_block : (NEWLINE* INDENT*) 'then' (NEWLINE* INDENT*) '{' NEWLINE* body NEWLINE* '}'
           ;

else_block : (NEWLINE* INDENT*) 'else' (NEWLINE* INDENT*) '{' NEWLINE* body NEWLINE* '}'
           ;


// Math expression rules
expression
    : binary_expression
    ;

binary_expression
    : term operator term
    | binary_expression operator term
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
    : type_member_access
    | function_call
    | ID
    | array_index
    | INT
    | NOVALUE
    | string_rule
    | '(' expression ')'
    ;


type : (INTTYPE | STRINGTYPE | COLLECTIONTYPE | ID | VOID ) ;
operator : ('*' | '/' |'-'|'+'| '>' | '<' | '=');
