#! /bin/bash
clear
echo '-----------------------------------'
echo pushing commit "$1" to master branch
echo '-----------------------------------'
git add .
git commit -m "$1" -a
git push  
