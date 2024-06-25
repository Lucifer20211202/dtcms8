<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员日志'},{title:'编辑消息'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="消息内容" name="info">
					<div class="tab-content">
						<el-form-item prop="userId" label="会员账户">
							<dt-member-select v-model="datas.model.userId"></dt-member-select>
						</el-form-item>
						<el-form-item prop="title" label="消息标题">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="content" label="消息内容">
							<dt-editor mini v-model="datas.model.content" placeholder="请输入内容"></dt-editor>
						</el-form-item>
						<el-form-item label="发送时间" v-if="datas.model.id>0">
							{{datas.model.addTime}}
						</el-form-item>
						<el-form-item label="阅读时间" v-if="datas.model.id>0 && datas.model.isRead>0">
							{{datas.model.readTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">提交保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtMemberSelect from '../../../components/control/DtMemberSelect.vue'
	import DtEditor from '../../../components/control/DtEditor.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			id: 0,
			userId: null,
			userName: null,
			title: null,
			content: null,
			addTime: null,
			isRead: 0,
			readTime: null
		},
		rules: {
			title: [
				{ required: true, message: '请输入消息标题', trigger: 'blur' }
			],
			content: [
				{ required: true, message: '请输入消息内容', trigger: 'blur' }
			],
			userId: [
				{ required: true, message: '请选择接会员', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/member/message/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/member/message/${datas.model.id}`,
						data: datas.model,
						successMsg: '消息已修改成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/member/message/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/member/message',
						data: datas.model,
						successMsg: '消息已发送成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	
	//运行初始化
	initData()
</script>