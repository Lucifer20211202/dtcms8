<template>
	<div class="mainbody">
		<dt-location :data="[{title:'应用管理'},{title:'留言反馈'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="留言信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否显示">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item prop="siteId" label="所属站点">
							<el-select v-model="datas.model.siteId" placeholder="请选择...">
								<el-option v-for="item in datas.siteList"
										   :key="item.id"
										   :label="item.title"
										   :value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="content" label="留言内容">
							<el-input type="textarea" :rows="5" v-model="datas.model.content" placeholder="必填，用户留言信息"></el-input>
						</el-form-item>
						<el-form-item prop="replyContent" label="回复内容" v-if="datas.model.id>0">
							<el-input type="textarea" :rows="5" v-model="datas.model.replyContent" placeholder="管理员回复内容"></el-input>
						</el-form-item>
						<el-form-item label="留言时间" v-if="datas.model.id>0">
							{{datas.model.addBy}} {{datas.model.addTime}}
						</el-form-item>
						<el-form-item label="回复时间" v-if="datas.model.replyTime">
							{{datas.model.replyBy}} {{datas.model.replyTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
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
		siteList: [],
		model: {
			id: 0,
			siteId: null,
			content: null,
			replyContent: null,
			status: null,
			addBy: null,
			addTime: null,
			replyBy: null,
			replyTime: null
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			content: [
				{ required: true, message: '请输入留言内容', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/feedback/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
		//加载站点列表
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		})
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
						url: `/admin/feedback/${datas.model.id}`,
						data: datas.model,
						successMsg: '回复留言反馈已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/apply/feedback/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/feedback',
						data: datas.model,
						successMsg: '新增留言反馈已成功',
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